from PIL import Image, ImageDraw, ImageFont
import os, math
base='/mnt/data/wm6ui/MtProto.WM6.ClientApp'
res=os.path.join(base,'Resources')
os.makedirs(res, exist_ok=True)
try:
    font=ImageFont.truetype('DejaVuSans-Bold.ttf', 18)
    small=ImageFont.truetype('DejaVuSans.ttf', 12)
except:
    font=small=None
BLUE=(55,145,210); DARK=(28,78,124); WHITE=(255,255,255); LIGHT=(230,244,252); GREY=(90,110,125)
def save_icon(name, symbol, bg=BLUE):
    im=Image.new('RGBA',(32,32),(0,0,0,0)); d=ImageDraw.Draw(im)
    d.rounded_rectangle([1,1,30,30], radius=7, fill=bg, outline=DARK)
    # custom shapes
    if symbol=='paper':
        d.polygon([(8,16),(25,7),(20,25),(15,18)], fill=WHITE)
        d.line([(15,18),(19,24)], fill=LIGHT, width=2)
    elif symbol=='gear':
        d.ellipse([8,8,24,24], outline=WHITE, width=3); d.ellipse([13,13,19,19], fill=WHITE)
        for a in range(0,360,45):
            x=16+math.cos(math.radians(a))*10; y=16+math.sin(math.radians(a))*10
            d.line([(16,16),(x,y)], fill=WHITE, width=2)
    elif symbol=='chat':
        d.rounded_rectangle([6,8,26,21], radius=4, fill=WHITE)
        d.polygon([(12,21),(10,27),(18,21)], fill=WHITE)
        d.ellipse([11,13,14,16], fill=BLUE); d.ellipse([17,13,20,16], fill=BLUE)
    elif symbol=='user':
        d.ellipse([11,7,21,17], fill=WHITE); d.rounded_rectangle([8,18,24,27], radius=6, fill=WHITE)
    elif symbol=='lock':
        d.rounded_rectangle([8,14,24,27], radius=3, fill=WHITE)
        d.arc([10,5,22,19], 180, 360, fill=WHITE, width=3)
        d.rectangle([10,12,22,16], fill=WHITE)
        d.ellipse([14,19,18,23], fill=BLUE)
    elif symbol=='plug':
        d.line([10,8,22,20], fill=WHITE, width=4); d.line([7,11,12,6], fill=WHITE, width=2); d.line([20,25,25,20], fill=WHITE, width=2)
    elif symbol=='key':
        d.ellipse([6,12,17,23], outline=WHITE, width=3); d.line([16,17,27,17], fill=WHITE, width=3); d.line([23,17,23,22], fill=WHITE, width=2)
    else:
        d.text((8,6), symbol, font=font, fill=WHITE)
    im.save(os.path.join(res,name+'.png'))
for name,sym in [('appicon','paper'),('send','paper'),('settings','gear'),('chat','chat'),('user','user'),('lock','lock'),('connect','plug'),('key','key')]: save_icon(name,sym)
# splash
im=Image.new('RGB',(240,320),(246,250,253)); d=ImageDraw.Draw(im)
d.rectangle([0,0,240,54], fill=BLUE)
d.text((56,17),'Pocket MTProto', font=font, fill=WHITE)
icon=Image.open(os.path.join(res,'appicon.png')).convert('RGBA').resize((40,40))
im.paste(icon,(10,7),icon)
d.rounded_rectangle([18,92,222,210], radius=14, fill=(255,255,255), outline=(205,220,230))
d.text((38,112),'WM6 Telegram-style', font=font, fill=DARK)
d.text((38,145),'Client shell for MTProto', font=small, fill=GREY)
d.text((38,168),'Original assets, no official', font=small, fill=GREY)
d.text((38,188),'Telegram branding included.', font=small, fill=GREY)
im.save(os.path.join(res,'splash.png'))
# ico
Image.open(os.path.join(res,'appicon.png')).save(os.path.join(res,'appicon.ico'), sizes=[(32,32),(16,16)])
