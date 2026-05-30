# MtProto.WM6

MTProto 2.0 client skeleton for Windows Mobile 6 / .NET Compact Framework 3.5.

## Added in this build

- Generic TL registry and fallback object parser.
- `rpc_result` / `rpc_error` parsing.
- `msg_container` parsing.
- DC migration helper for `*_MIGRATE_X` errors.
- 2FA entry points: `account.getPassword`, `auth.checkPassword`, and SRP proof carrier.
- Test-DC smoke harness.
- Session save/load.

## Reality check

This project is still not a finished Telegram client. The TL generator can produce broad schema classes, but you still need to generate and include the current schema before expecting rich typed responses. Telegram's current public schema is Layer 214 at the time this package was produced.

The 2FA SRP proof is intentionally isolated in `Runtime/PasswordSrp.cs`. `auth.checkPassword` is wired, but full password proof generation must validate the current `account.Password` KDF algorithm returned by Telegram before use.

## Typical flow

```csharp
MtProtoClient client = new MtProtoClient();
DcEndpoint dc = DcOptions.Test(1);
client.Connect(dc.Host, dc.Port);

object sent = client.AuthSendCode("+10000000000", apiId, apiHash, true);
object signed = client.AuthSignIn("+10000000000", phoneCodeHash, "22222", true);
```

## Files of interest

- `Runtime/Rpc.cs`
- `Runtime/MessageContainer.cs`
- `Runtime/DcOptions.cs`
- `Runtime/PasswordSrp.cs`
- `Generated/TLBootstrap.cs`
- `Tools/tlgen.py`
