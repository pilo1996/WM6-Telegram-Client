#!/usr/bin/env python3
import re, sys, zlib

PRIMITIVES = {
    'int': ('int', 'ReadInt', 'WriteInt'),
    'long': ('long', 'ReadLong', 'WriteLong'),
    'double': ('double', None, None),
    'string': ('string', 'ReadString', 'WriteString'),
    'bytes': ('byte[]', 'ReadBytes', 'WriteBytes'),
    'Bool': ('bool', None, None),
    'true': ('bool', None, None),
}

SKIP_PREFIXES = ('//', '---')

def clean(line):
    line = line.strip()
    if not line or line.startswith(SKIP_PREFIXES): return None
    if line.startswith('---'): return None
    return line

def crc_id(defn):
    # Telegram constructor IDs are CRC32 over normalized TL lines when no explicit #id exists.
    return zlib.crc32(defn.encode('ascii')) & 0xffffffff

def cs_name(name):
    return ''.join(p[:1].upper()+p[1:] for p in re.split(r'[._]', name) if p).replace('<','').replace('>','')

def parse_arg(tok):
    if ':' not in tok: return None
    n,t = tok.split(':',1)
    if n == 'flags': return ('flags', '#', None, None)
    flag = None
    m = re.match(r'flags\.(\d+)\?(.+)', t)
    if m:
        flag = int(m.group(1)); t = m.group(2)
    return (n, t, flag, None)

def cs_type(t):
    if t.startswith('Vector<') or t.startswith('Vector <'):
        inner = t[t.find('<')+1:t.rfind('>')].strip()
        return cs_type(inner) + '[]'
    return PRIMITIVES.get(t, (cs_name(t), None, None))[0]

def emit(lines):
    out=[]
    out.append('using MtProto.WM6.TL;')
    out.append('namespace MtProto.WM6.GeneratedFull {')
    for raw in lines:
        line=clean(raw)
        if not line or '=' not in line or line.startswith('vector#'): continue
        left,right=line.split('=',1)
        result=right.replace(';','').strip()
        parts=left.strip().split()
        head=parts[0]
        if '#' in head:
            name,hid=head.split('#',1); cid=int(hid,16)
        else:
            name=head; cid=crc_id(left.strip()+' = '+result)
        args=[parse_arg(p) for p in parts[1:]]
        args=[a for a in args if a]
        cls=cs_name(name)
        out.append('  public sealed class %s : TLObject {'%cls)
        out.append('    public const int ID = unchecked((int)0x%08x);'%cid)
        out.append('    public override int ConstructorId { get { return ID; } }')
        for n,t,flag,_ in args:
            if t=='#': out.append('    public int %s;'%cs_name(n)); continue
            out.append('    public %s %s;'%(cs_type(t), cs_name(n)))
        out.append('    public override void Serialize(TLBinaryWriter w) {')
        for n,t,flag,_ in args:
            cn=cs_name(n)
            prefix='' if flag is None else 'if ((Flags & (1 << %d)) != 0) '%flag
            if t=='#': out.append('      w.WriteInt(%s);'%cn)
            elif t.startswith('Vector'):
                inner=t[t.find('<')+1:t.rfind('>')].strip()
                if inner == 'int': out.append('      %sw.WriteIntVector(%s);'%(prefix,cn))
                elif inner == 'long': out.append('      %sw.WriteLongVector(%s);'%(prefix,cn))
                else: out.append('      %sw.WriteObjectVector(%s);'%(prefix,cn))
            elif t in PRIMITIVES and PRIMITIVES[t][2]:
                out.append('      %sw.%s(%s);'%(prefix,PRIMITIVES[t][2],cn))
            elif t=='true':
                pass
            else:
                out.append('      %sw.WriteObject(%s);'%(prefix,cn))
        out.append('    }')
        out.append('    public static %s Deserialize(TLBinaryReader r) {'%cls)
        out.append('      %s x = new %s();'%(cls,cls))
        for n,t,flag,_ in args:
            cn=cs_name(n)
            prefix='' if flag is None else 'if ((x.Flags & (1 << %d)) != 0) '%flag
            if t=='#': out.append('      x.%s = r.ReadInt();'%cn)
            elif t.startswith('Vector'):
                inner=t[t.find('<')+1:t.rfind('>')].strip()
                if inner == 'int': out.append('      %sx.%s = r.ReadIntVector();'%(prefix,cn))
                elif inner == 'long': out.append('      %sx.%s = r.ReadLongVector();'%(prefix,cn))
                else: out.append('      %sx.%s = r.ReadObjectVector();'%(prefix,cn))
            elif t in PRIMITIVES and PRIMITIVES[t][1]:
                out.append('      %sx.%s = r.%s();'%(prefix,cn,PRIMITIVES[t][1]))
            elif t=='true':
                out.append('      x.%s = true;'%cn)
            else:
                out.append('      %sx.%s = (%s)TLRegistry.Deserialize(r);'%(prefix,cn,cs_type(t)))
        out.append('      return x;')
        out.append('    }')
        out.append('  }')
    out.append('}')
    return '\n'.join(out)

if __name__ == '__main__':
    data=sys.stdin.readlines() if len(sys.argv)==1 else open(sys.argv[1],encoding='utf-8').readlines()
    print(emit(data))
