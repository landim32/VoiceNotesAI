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