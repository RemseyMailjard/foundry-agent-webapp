import {
  type BrandVariants,
  createDarkTheme,
  createLightTheme,
  type Theme,
} from "@fluentui/react-components";

// M365 Buddy brand ramp, anchored on the product's primary blue (#2b62e8) at step 80
// (Fluent's interactive/button anchor). See m365buddy.nl/DESIGN.md for the source palette —
// this ramp is generated from that primary color, not copied from Microsoft's own branding.
const brandColors: BrandVariants = {
  10: "#010409",
  20: "#040B1C",
  30: "#061538",
  40: "#0A2159",
  50: "#0E2E7D",
  60: "#123CA3",
  70: "#164BCD",
  80: "#2B62E8",
  90: "#4F7CEC",
  100: "#6990EF",
  110: "#81A2F1",
  120: "#98B2F4",
  130: "#ADC2F6",
  140: "#C3D2F6",
  150: "#D8E1F7",
  160: "#EAEFFB",
};

// M365 Buddy uses Parkinsans for its friendly, rounded typographic personality
// (see DESIGN.md "Typography"). Falls back to Fluent's default stack if the font fails to load.
const fontFamilyBase =
  "'Parkinsans', 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, Helvetica, Arial, sans-serif";

export const lightTheme: Theme = {
  ...createLightTheme(brandColors),
  fontFamilyBase,
};

export const darkTheme: Theme = {
  ...createDarkTheme(brandColors),
  fontFamilyBase,
  // Enhanced dark mode foreground colors for better readability
  colorBrandForeground1: brandColors[110],
  colorBrandForeground2: brandColors[120],
  colorBrandForegroundLink: brandColors[140],
};

/**
 * M365 Buddy's amber accent — the one dedicated call-to-action color, deliberately kept
 * separate from the blue brand ramp above (see DESIGN.md "Colors"). Not part of Fluent's
 * token system; import directly where a primary CTA button needs it.
 */
export const brandAccent = {
  amber: "#fca636",
  amberLight: "#fdce90",
  amberDark: "#ca852b",
  ink: "#08122b", // text color on top of amber — passes AA contrast, unlike white-on-amber
};
