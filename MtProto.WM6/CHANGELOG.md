# Changelog

## v0.3

Implemented the next integration block:

1. Generic TL registry and generated-schema pipeline.
2. `rpc_result` and `rpc_error` handling.
3. `msg_container` parser.
4. DC migration helper for `PHONE_MIGRATE_X`, `NETWORK_MIGRATE_X`, `USER_MIGRATE_X`, etc.
5. 2FA API wiring via `account.getPassword` and `auth.checkPassword` with SRP proof carrier.
6. Test DC smoke harness.

Known limitation: full SRP proof calculation is isolated and must be completed against Telegram's current password KDF object before real 2FA login.

## v0.4

Integrated the next missing block after v0.3:

1. Service message parsing for `bad_server_salt`, `bad_msg_notification`, `new_session_created`, `pong`, `gzip_packed` carrier, and retry on `bad_server_salt`.
2. 2FA SRP proof calculation for Telegram's SHA256/SHA256/PBKDF2-HMAC-SHA512/SHA256/modPow KDF.
3. `account.password` carrier parsing for current password login fields.
4. `AuthCheckPassword(AccountPassword, string, bool)` convenience overload.
5. Better generated TL vector support for `Vector<int>`, `Vector<long>`, and object vectors.

Remaining validation needed before production: run against Telegram test DC with current schema constructors and verify SRP values with known fixtures.
