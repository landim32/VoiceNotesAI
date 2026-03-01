# Voice Notes AI — Brand Implementation Guide

## Brand Overview

- **Brand Name:** Voice Notes AI
- **Description:** App para criação de anotações em voz e controle de prompts

## Color Palette

| Role | Hex Code | Preview |
|------|----------|---------|
| Primary | `#F07040` | ![#F07040](https://via.placeholder.com/20/F07040/F07040) |
| Secondary | `#E0E0E0` | ![#E0E0E0](https://via.placeholder.com/20/E0E0E0/E0E0E0) |
| Accent | `#101020` | ![#101020](https://via.placeholder.com/20/101020/101020) |
| Palette 4 | `#F07000` | ![#F07000](https://via.placeholder.com/20/F07000/F07000) |
| Palette 5 | `#80C0D0` | ![#80C0D0](https://via.placeholder.com/20/80C0D0/80C0D0) |

### CSS Variables

```css
:root {
  --color-primary: #F07040;
  --color-secondary: #E0E0E0;
  --color-accent: #101020;
  --color-palette-4: #F07000;
  --color-palette-5: #80C0D0;
}
```

## Logo Assets

| File | Variant | Recommended Usage |
|------|---------|-------------------|
| `logo-base.png` | Square | App icons, favicons, profile avatars, square placements |
| `logo-icon.png` | Icon | App icons (no text), favicons, loading screens, watermarks |
| `logo-horizontal-light.png` | HorizontalLight | Headers, navigation bars, email signatures (light backgrounds) |
| `logo-vertical-light.png` | VerticalLight | Splash screens, about pages, print (light backgrounds) |
| `logo-base-dark.png` | VerticalDark | Splash screens, about pages (dark backgrounds/dark mode) |
| `logo-icon-dark.png` | IconDark | Same as Icon but for dark backgrounds/dark mode |
| `logo-horizontal-dark.png` | HorizontalDark | Headers, navigation bars (dark backgrounds/dark mode) |
| `logo-vertical-dark.png` | VerticalDark | Splash screens, about pages (dark backgrounds/dark mode) |

---

## AI Implementation Prompts

Use the prompts below with an AI assistant to implement the brand's visual identity in your project.
Each prompt is also saved as an individual file in the `prompts/` folder for easy copy-paste.

- `prompts/prompt-react.md` — Frontend React (Web)
- `prompts/prompt-desktop.md` — Desktop Application (Windows — WPF/WinUI)
- `prompts/prompt-mobile.md` — Mobile Application (React Native / Flutter)

---

### Prompt 1: Frontend React (Web)

```
I need you to implement a complete visual layout for a React web application for the brand "Voice Notes AI".

BRAND DESCRIPTION:
App para criação de anotações em voz e controle de prompts

COLOR PALETTE:
- Primary: #F07040
- Secondary: #E0E0E0
- Accent: #101020
- Additional 4: #F07000
- Additional 5: #80C0D0

LOGO FILES AVAILABLE:
`logo-base.png`, `logo-icon.png`, `logo-horizontal-light.png`, `logo-vertical-light.png`, `logo-base-dark.png`, `logo-icon-dark.png`, `logo-horizontal-dark.png`, `logo-vertical-dark.png`

REQUIREMENTS:
1. Create a responsive layout with:
   - Header/Navbar using the horizontal logo variant (logo-horizontal-light.png for light mode, logo-horizontal-dark.png for dark mode)
   - Hero section with the brand colors as gradient or solid background
   - Footer with the vertical or horizontal logo variant
2. Implement a complete design system using the color palette above:
   - Primary color for main CTAs, links, and active states
   - Secondary color for secondary buttons, borders, and subtle backgrounds
   - Accent color for highlights, badges, and notifications
3. Support dark mode/light mode toggle:
   - Light mode: use the "-light" logo variants on white/light backgrounds
   - Dark mode: use the "-dark" logo variants on dark backgrounds
4. Use the icon variant (logo-icon.png) as the favicon and PWA icon
5. Typography: choose a modern sans-serif font that complements the brand
6. Create reusable components: Button, Card, Input, Badge, Avatar — all using the brand colors
7. Use CSS variables (custom properties) for all brand colors so they can be easily changed
8. Make the layout fully responsive (mobile-first approach)
9. Use Tailwind CSS or styled-components — pick whichever you prefer, but be consistent
10. Include loading/splash screen with the icon logo centered on a brand-colored background
```

---

### Prompt 2: Desktop Application (Windows — WPF/WinUI)

```
I need you to implement a complete visual layout for a Windows desktop application for the brand "Voice Notes AI".

BRAND DESCRIPTION:
App para criação de anotações em voz e controle de prompts

COLOR PALETTE:
- Primary: #F07040
- Secondary: #E0E0E0
- Accent: #101020
- Additional 4: #F07000
- Additional 5: #80C0D0

LOGO FILES AVAILABLE:
`logo-base.png`, `logo-icon.png`, `logo-horizontal-light.png`, `logo-vertical-light.png`, `logo-base-dark.png`, `logo-icon-dark.png`, `logo-horizontal-dark.png`, `logo-vertical-dark.png`

REQUIREMENTS:
1. Create a professional desktop application layout with:
   - Title bar or custom chrome with the horizontal logo (logo-horizontal-light.png)
   - Sidebar navigation using the brand's primary and secondary colors
   - Main content area with cards and data presentation
   - Status bar at the bottom
2. Implement a ResourceDictionary (XAML) with:
   - All brand colors as SolidColorBrush resources
   - Styles for Button, TextBox, ComboBox, ListBox, DataGrid — all themed with brand colors
   - Primary color for main actions and selected states
   - Secondary color for borders, panel backgrounds, and hover states
   - Accent color for highlights and notifications
3. Support Windows light/dark theme:
   - Detect system theme and switch logo variants accordingly
   - Light theme: use "-light" logo variants
   - Dark theme: use "-dark" logo variants
4. Window icon: use the icon variant (logo-icon.png) as the .ico file
5. Splash screen on startup: centered icon logo on a background with the primary color
6. Use the recommended WinUI 3 or WPF framework
7. Include a consistent margin/padding system (8px base grid)
8. Navigation should support: sidebar collapse, breadcrumbs, and tab-based content
```

---

### Prompt 3: Mobile Application (React Native / Flutter)

```
I need you to implement a complete visual layout for a mobile application for the brand "Voice Notes AI".

BRAND DESCRIPTION:
App para criação de anotações em voz e controle de prompts

COLOR PALETTE:
- Primary: #F07040
- Secondary: #E0E0E0
- Accent: #101020
- Additional 4: #F07000
- Additional 5: #80C0D0

LOGO FILES AVAILABLE:
`logo-base.png`, `logo-icon.png`, `logo-horizontal-light.png`, `logo-vertical-light.png`, `logo-base-dark.png`, `logo-icon-dark.png`, `logo-horizontal-dark.png`, `logo-vertical-dark.png`

REQUIREMENTS:
1. Create a mobile-first layout with:
   - Splash/launch screen with centered icon logo (logo-icon.png) on a primary-colored background
   - Top app bar / navigation bar with the horizontal logo (logo-horizontal-light.png or logo-horizontal-dark.png)
   - Bottom tab navigation with icons using the brand's primary color for active state
   - Pull-to-refresh with brand-colored indicator
2. Implement a complete theme/design system:
   - Primary color for main CTAs, FABs, selected tabs, and active states
   - Secondary color for secondary actions, card borders, and input outlines
   - Accent color for badges, notifications, and small highlights
3. Support light mode and dark mode:
   - Light mode: white/light backgrounds, dark text, "-light" logo variants
   - Dark mode: dark backgrounds, light text, "-dark" logo variants
   - Follow system preference by default, with manual toggle option
4. App icon: use the icon variant (logo-icon.png and logo-icon-dark.png) to generate adaptive icons (Android) and app icons (iOS)
5. Create reusable themed components: Button (primary/secondary/outline), Card, Input, Avatar, Badge, Chip, Dialog
6. Typography: use a system font or a modern sans-serif that complements the brand
7. Consistent spacing system (4px or 8px base unit)
8. Animations: subtle fade/slide transitions using the brand's accent color for loading indicators
9. Include onboarding screens (2-3 slides) showcasing the brand with the vertical logo variant
10. Handle safe areas, notch, and different screen sizes properly
```

---

> Generated by **AI Logo Maker** — Use these prompts with any AI coding assistant (Claude, ChatGPT, Copilot, etc.) to implement your brand's visual identity consistently across platforms.
