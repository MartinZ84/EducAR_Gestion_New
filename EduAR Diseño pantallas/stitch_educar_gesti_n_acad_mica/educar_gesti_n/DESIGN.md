---
name: EducAR Gestión
colors:
  surface: '#f7f9fe'
  surface-dim: '#d8dadf'
  surface-bright: '#f7f9fe'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f1f4f9'
  surface-container: '#eceef3'
  surface-container-high: '#e6e8ed'
  surface-container-highest: '#e0e2e7'
  on-surface: '#181c20'
  on-surface-variant: '#414754'
  inverse-surface: '#2d3135'
  inverse-on-surface: '#eff1f6'
  outline: '#727785'
  outline-variant: '#c1c6d6'
  surface-tint: '#005bc0'
  primary: '#005bbf'
  on-primary: '#ffffff'
  primary-container: '#1a73e8'
  on-primary-container: '#ffffff'
  inverse-primary: '#adc7ff'
  secondary: '#005ac1'
  on-secondary: '#ffffff'
  secondary-container: '#4d8efe'
  on-secondary-container: '#00285c'
  tertiary: '#006d2c'
  on-tertiary: '#ffffff'
  tertiary-container: '#008939'
  on-tertiary-container: '#ffffff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#d8e2ff'
  primary-fixed-dim: '#adc7ff'
  on-primary-fixed: '#001a41'
  on-primary-fixed-variant: '#004493'
  secondary-fixed: '#d8e2ff'
  secondary-fixed-dim: '#adc6ff'
  on-secondary-fixed: '#001a41'
  on-secondary-fixed-variant: '#004494'
  tertiary-fixed: '#89fa9b'
  tertiary-fixed-dim: '#6ddd81'
  on-tertiary-fixed: '#002108'
  on-tertiary-fixed-variant: '#005320'
  background: '#f7f9fe'
  on-background: '#181c20'
  surface-variant: '#e0e2e7'
typography:
  display-lg:
    fontFamily: Roboto Flex
    fontSize: 57px
    fontWeight: '400'
    lineHeight: 64px
    letterSpacing: -0.25px
  headline-lg:
    fontFamily: Roboto Flex
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
  headline-lg-mobile:
    fontFamily: Roboto Flex
    fontSize: 28px
    fontWeight: '600'
    lineHeight: 36px
  title-lg:
    fontFamily: Roboto Flex
    fontSize: 22px
    fontWeight: '500'
    lineHeight: 28px
  title-md:
    fontFamily: Roboto Flex
    fontSize: 16px
    fontWeight: '500'
    lineHeight: 24px
    letterSpacing: 0.15px
  body-lg:
    fontFamily: Roboto Flex
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
    letterSpacing: 0.5px
  body-md:
    fontFamily: Roboto Flex
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
    letterSpacing: 0.25px
  label-lg:
    fontFamily: Roboto Flex
    fontSize: 14px
    fontWeight: '500'
    lineHeight: 20px
    letterSpacing: 0.1px
  label-sm:
    fontFamily: Roboto Flex
    fontSize: 11px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.5px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 32px
  margin-mobile: 16px
  gutter-mobile: 12px
---

## Brand & Style
The design system is built for an academic environment, prioritizing clarity, trust, and efficiency for primary school educators. The personality is professional and institutional yet approachable, ensuring teachers can manage complex data without cognitive overload.

The visual style follows a **Corporate Modern** approach, heavily influenced by **Material Design 3 (MD3)**. It utilizes high-quality whitespace, a structured grid, and a focus on functional clarity. The goal is to evoke a sense of reliability and academic prestige while maintaining the ease of use expected from a modern mobile application.

## Colors
The palette is rooted in **Institutional Blue**, representing stability and intelligence. 
- **Primary:** The core brand color used for key actions and active states.
- **Surface & Background:** A very light blue-tinted neutral (#F8FAFF) is used for the main background to reduce eye strain, while pure white is reserved for cards and elevated containers.
- **Semantic Colors:** Green is used for "Presente" (Attendance) or "Aprobado" (Passed) statuses, while standard MD3 error reds are used for alerts.

## Typography
This design system uses **Roboto Flex** for its exceptional legibility and mechanical precision, which aligns with the academic nature of the product. 
- **Hierarchy:** Headlines are prominent to help teachers quickly identify which section of the "Aula" or "Legajo" they are viewing. 
- **Language Support:** All type scales are optimized for Spanish (Español), accounting for character descenders and slightly longer word lengths.
- **Function:** `Title-MD` is the standard for list items and student names, while `Label-LG` is used for form headers and interactive chips.

## Layout & Spacing
The layout follows a **Fluid Grid** model optimized for mobile-first interactions. 
- **Rhythm:** A 4dp/8dp base increment is used to ensure all elements align to the Material grid.
- **Margins:** 16px lateral margins are standard for mobile views.
- **Touch Targets:** All interactive elements maintain a minimum height of 48px to ensure ease of use for teachers while moving through the classroom.
- **Structure:** Content is organized into clear vertical stacks. On tablets, the system transitions to a multi-pane layout (List-Detail view) to utilize the extra horizontal space for student profiles.

## Elevation & Depth
Depth is communicated through **Tonal Layers** and **Soft Ambient Shadows**. 
- **Level 0 (Background):** #F8FAFF.
- **Level 1 (Cards):** Surface color (White) with a subtle 1px border (#C4C7C5) or a soft shadow (Blur: 4px, Y: 2px, Opacity: 8% Black).
- **Level 2 (Floating Action Buttons):** Primary color with a more pronounced shadow (Blur: 8px, Y: 4px, Opacity: 12% Black) to indicate the primary entry point for "Carga de Notas" or "Asistencia".
- **Interaction:** Upon press, elements gain a subtle tonal overlay (state layer) to provide immediate haptic feedback.

## Shapes
The shape language is friendly and modern. 
- **Containers/Cards:** Use a 16px radius (`rounded-lg`) to create a soft, approachable container for student data.
- **Buttons:** Use fully rounded (pill-shaped) or 12px corners depending on the button's hierarchy.
- **Inputs:** Use an 8px radius for text fields to maintain a professional, structured feel.

## Components
- **Buttons:** Primary buttons use the Institutional Blue background with White text. Secondary buttons use an outlined style.
- **Cards (Tarjetas de Alumno):** High-radius (16px+) containers with `Title-MD` for names. Use color-coded chips for attendance status (e.g., "Presente" in Green, "Ausente" in Grey).
- **Input Fields:** Outlined Material fields with clear labels and helpful supporting text. Use the primary blue for active focus states.
- **Chips (Filtros):** Used for filtering by "Grado," "División," or "Materia." They should have an 8px radius and a light blue background when selected.
- **Lists:** Clean rows with Material Icons on the leading edge (e.g., `person` for student, `description` for grades) and a chevron on the trailing edge to indicate drill-down.
- **Navigation:** A Bottom Navigation Bar for quick access to "Inicio," "Alumnos," "Calificaciones," and "Perfil."