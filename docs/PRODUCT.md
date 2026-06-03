# Product Vision

## One-line description

TidyTop is a lightweight Windows desktop organizer that groups desktop items into clean, customizable SmartBoxes.

## Problem

Many users keep shortcuts, folders, installers, documents, and random files directly on the desktop. Over time the desktop becomes visually noisy and harder to use. Existing desktop organization tools can feel heavy, overcomplicated, or too tied to one specific workflow.

## Product goal

Make desktop cleanup feel instant, visual, and safe.

The first good version should let a user open TidyTop, see their real desktop items organized into boxes, move things where they want, save the layout, and trust that the layout comes back after restarting the app.

## Target user

- Windows users with cluttered desktops.
- Students, developers, office workers, gamers, and power users who keep many shortcuts.
- Users who want organization without replacing the whole desktop experience.

## MVP promise

The MVP should do one thing well:

> Organize real desktop items into visual boxes and persist that layout.

## Non-goals for v0.1

These are intentionally delayed:

- AI organization.
- Widgets.
- Wallpapers.
- Calendar integration.
- Notes.
- Cloud sync.
- macOS/Linux support.
- Advanced rules engine.
- Marketplace/themes.
- Full desktop replacement behavior.

## Naming language

Use **SmartBox** as the product/domain term.

Avoid using **Fence** in public UI or docs. It makes the app feel derivative and creates unnecessary comparison with existing products.

## Platform strategy

TidyTop is Windows-first for v0.1 because desktop icon behavior, shell integration, shortcuts, and global hotkeys are platform-specific.

Avalonia can still be kept as the UI framework, but cross-platform support should not be promised until each platform has real desktop integration code.
