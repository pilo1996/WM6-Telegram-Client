# UI Assets

All UI assets in this package are original generated images created for this sample project.

They are intentionally **not** Telegram official assets. Do not replace them with Telegram's logo or official artwork unless you have verified the relevant trademark and brand-use rules.

## Included files

| File | Purpose |
| --- | --- |
| `appicon.ico` | Windows Mobile application icon |
| `appicon.png` | Header icon |
| `splash.png` | Startup splash screen |
| `send.png` | Send button |
| `settings.png` | Settings button |
| `chat.png` | Chat/log button |
| `user.png` | Back/user identity button |
| `lock.png` | 2FA screen |
| `connect.png` | Login/connect action |
| `key.png` | Code/password action |

## Runtime loading

`UiTheme.LoadBitmap()` loads images from:

```text
<application folder>/Resources/<name>.png
```

The `.csproj` marks these files as content with `CopyToOutputDirectory=Always`.
