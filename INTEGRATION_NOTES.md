# Integration notes

## What was added in this package

- VS2008 solution file.
- Windows Mobile Forms client project.
- Login, code, 2FA, chat and settings forms.
- Diagnostic log service.
- Resilient MTProto wrapper for retry, DC migration and FLOOD_WAIT surfacing.
- README suitable for GitHub.

## Known compile caveat

This solution is authored for Visual Studio 2008 and .NET Compact Framework 3.5. Modern Linux/Mono build tools are not a reliable validator for Smart Device project metadata.

## Next engineering task

Regenerate and type the full Telegram schema so `auth.sentCode` and `auth.authorization` parse as first-class C# objects. The current UI deliberately fails visibly when `phone_code_hash` cannot be extracted.
