# MtProto.WM6 Mobile Client

Windows Mobile 6 demo client for the `MtProto.WM6` library, built as a Visual Studio 2008 solution for .NET Compact Framework 3.5. Yes, this targets WM6. Humanity made choices.

## What is included

- `MtProto.WM6`: MTProto library source.
- `MtProto.WM6.ClientApp`: Windows Mobile Forms client.
- `MtProto.WM6.MobileClient.sln`: Visual Studio 2008 solution.
- `Tools/tlgen.py`: TL schema generator helper.
- Test/smoke harness classes for crypto and Telegram test DC validation.

## App screens

The sample client includes these forms:

- `LoginForm`: phone-number login and `auth.sendCode`.
- `CodeForm`: SMS/app code flow and `auth.signIn`.
- `Password2FAForm`: cloud password flow using `account.getPassword` and `auth.checkPassword`.
- `ChatForm`: minimal `messages.sendMessage` test UI.
- `SettingsForm`: API credentials, DC host, port, test/prod mode, diagnostics.

## Requirements

- Visual Studio 2008 Professional or Team System.
- Windows Mobile 6 SDK / Windows Mobile 6 Professional SDK.
- .NET Compact Framework 3.5.
- Device emulator or real WM6 device.
- Telegram API credentials from your Telegram developer account.

## Opening the project

1. Extract the ZIP.
2. Open `MtProto.WM6.MobileClient.sln` in Visual Studio 2008.
3. Select a Windows Mobile 6 emulator/device target.
4. Build the solution.
5. Open the app settings screen and set:
   - `ApiId`
   - `ApiHash`
   - `Host`
   - `Port`
   - `Test DC` enabled or disabled

Default settings target Telegram test DC 2:

```text
Host: 149.154.167.40
Port: 443
Test DC: true
```

## Current integration status

This is a working integration scaffold, not a guaranteed production Telegram client. The project includes the pieces needed to wire the UI to MTProto, but Telegram compatibility still depends on validating the generated TL layer and crypto flows against live test DCs.

Implemented integration points:

- Real app project for WM6 Forms.
- Login, code, 2FA, chat and settings screens.
- MTProto session load/save.
- Diagnostic logging to `\My Documents\MtProtoWM6\client.log`.
- Basic retry/reconnect wrapper.
- DC migration handling through `*_MIGRATE_X` RPC errors.
- `FLOOD_WAIT_X` detection and user-facing error.
- Test DC-oriented settings.
- TL generator included for full schema regeneration.

## Important limitations

### Test DC validation

Run the smoke harness against Telegram test DC before relying on login or message sending. The project contains `Tests/TestDcSmoke.cs`, but you must execute it from a device/emulator environment with network access.

### SRP 2FA verification

The SRP implementation is wired into the 2FA form. It still needs final validation against accounts with cloud password enabled on Telegram test DC. Cryptography without test vectors is just confidence cosplay.

### FLOOD_WAIT handling

The app detects `FLOOD_WAIT_X` and reports it. It does not silently sleep and retry because mobile UIs freezing for 20 minutes is a crime, even by 2009 standards.

### Reconnect and timeout handling

`ResilientMtClient` performs a simple reconnect and retry. Production use should add:

- socket receive/send timeout support,
- exponential backoff,
- cancellation from UI,
- network state detection.

### UI flow for SMS/app code

The UI supports manual code entry. If `auth.sentCode` is not fully typed by the generated TL schema, `phone_code_hash` extraction will fail and the schema must be regenerated.

### Diagnostic logging

Logs are written to:

```text
\My Documents\MtProtoWM6\client.log
```

Never log phone codes, cloud passwords, raw auth keys, or full encrypted payloads.

### Secure auth_key storage

The sample currently uses `SessionStore` from the library. This is acceptable for development only. Before production, replace it with a device-bound encrypted store using one of:

- native CryptoAPI wrapper,
- OEM certificate store,
- encrypted SQL CE database,
- custom hardware/device secret integration.

Do not ship plaintext `auth_key`. That would be handing the house keys to the raccoon and calling it architecture.

### Full Telegram TL schema

The checked-in generated layer is still partial. For a complete client, regenerate from the official Telegram `.tl` schema with `Tools/tlgen.py`, then add typed parsers for required response objects.

Minimum objects to verify for this app:

- `auth.sentCode`
- `auth.authorization`
- `account.password`
- `updates` / `updateShortSentMessage`
- `rpc_error`
- `msg_container`
- `bad_server_salt`

### Real WM6 testing

Test both emulator and physical device if possible. Emulator networking and old TLS/socket stacks can differ. Naturally, because debugging one obsolete platform was apparently too merciful.

## Suggested validation checklist

- [ ] Build solution in VS2008.
- [ ] Deploy to WM6 emulator.
- [ ] Open settings and configure test DC credentials.
- [ ] Run login with Telegram test account.
- [ ] Confirm `auth.sendCode` returns typed `auth.sentCode`.
- [ ] Confirm `auth.signIn` returns typed authorization or 2FA requirement.
- [ ] Validate SRP 2FA with cloud-password account.
- [ ] Send message to test peer.
- [ ] Trigger and verify DC migration handling.
- [ ] Trigger and verify `FLOOD_WAIT_X` handling.
- [ ] Inspect logs for missing TL constructor IDs.
- [ ] Replace session storage before production use.

## Repository layout

```text
MtProto.WM6.MobileClient.sln
MtProto.WM6/
  Client/
  Crypto/
  Generated/
  Runtime/
  Session/
  TL/
  Tests/
  Tools/
  Transport/
MtProto.WM6.ClientApp/
  Forms are implemented as code-only .cs files
  Services/
```

## License

See `MtProto.WM6/LICENSE.txt`.

## Disclaimer

This project is an experimental Windows Mobile 6 MTProto client scaffold. It is not affiliated with Telegram. Use Telegram APIs according to Telegram's terms and validate everything against test DC before touching production accounts.

## UI assets and visual shell

This version includes a lightweight Windows Mobile 6 UI skin with original, project-owned assets:

- `Resources/appicon.ico` application icon
- `Resources/appicon.png` header icon
- `Resources/splash.png` splash screen
- `Resources/send.png`, `settings.png`, `chat.png`, `user.png`, `lock.png`, `connect.png`, `key.png`
- `UiTheme.cs` centralized colors, header drawing and icon-button helper
- `SplashForm.cs` startup screen before login

No official Telegram logo, trademarked artwork, or branded assets are bundled. The visual language is Telegram-inspired only in the broad “blue messenger” sense, because legal ambiguity is a terrible dependency manager.

### Windows Mobile graphics notes

The images are small PNG files copied to the output folder as content. This avoids `.resx` friction on Visual Studio 2008 / .NET Compact Framework projects and keeps deployment simple:

```text
MtProto.WM6.ClientApp/Resources/*.png
MtProto.WM6.ClientApp/Resources/appicon.ico
```

If a target device has trouble loading PNGs through `System.Drawing.Bitmap`, replace the PNGs with BMP files and adjust `UiTheme.LoadBitmap` accordingly.
