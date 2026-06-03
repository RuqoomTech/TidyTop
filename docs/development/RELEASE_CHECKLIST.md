# Release Checklist

## v0.1 release candidate requirements

Do not release v0.1 until every item below is true.

### Product flow

- [ ] User can see real desktop items.
- [ ] User can create a SmartBox.
- [ ] User can rename a SmartBox.
- [ ] User can delete a SmartBox.
- [ ] User can move an item into a SmartBox.
- [ ] User can move an item between SmartBoxes.
- [ ] User can move an item back to Other/unboxed.
- [ ] User can close and reopen the app without losing the layout.
- [ ] User can hide/show boxes quickly.

### Quality

- [ ] `dotnet build` passes.
- [ ] `dotnet test` passes.
- [ ] Layout save/load has tests.
- [ ] Corrupt layout file handling is tested.
- [ ] Manual Windows smoke test completed.

### Packaging

- [ ] Version number set.
- [ ] Release notes written.
- [ ] Portable build or installer produced.
- [ ] Known issues listed.

### Documentation

- [ ] README current status is accurate.
- [ ] `docs/STATUS.md` is updated.
- [ ] `docs/roadmap/TASKS.md` reflects done/pending tasks.
- [ ] `CHANGELOG.md` has release entry.
