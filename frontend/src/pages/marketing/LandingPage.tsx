import { makeStyles, tokens, Button, Title1, Body1, Text } from '@fluentui/react-components';
import { MarketingLayout } from './MarketingLayout';
import { brandAccent } from '../../config/themes';

// Fluent tokens don't expose a "brand dark" alias directly — reuse the brand ramp's step 60,
// matching DESIGN.md's "icon-tile" spec (primary-dark background).
const brandColorsIconTile = tokens.colorBrandBackground2Hover;

const useStyles = makeStyles({
  hero: {
    display: 'grid',
    gridTemplateColumns: '1.05fr 0.95fr',
    gap: '48px',
    alignItems: 'center',
    paddingTop: '32px',
    paddingBottom: '56px',
    '@media (max-width: 900px)': {
      gridTemplateColumns: '1fr',
    },
  },
  eyebrow: {
    display: 'inline-flex',
    alignItems: 'center',
    gap: '8px',
    padding: '6px 14px',
    borderRadius: tokens.borderRadiusCircular,
    backgroundColor: tokens.colorBrandBackground2,
    color: tokens.colorBrandForeground2,
    fontWeight: 600,
    fontSize: '13px',
    width: 'fit-content',
  },
  title: {
    marginTop: '20px',
    fontFamily: "'TRICK', 'Parkinsans', sans-serif",
    fontSize: '48px',
    lineHeight: 1.08,
    letterSpacing: '-0.02em',
    '@media (max-width: 600px)': {
      fontSize: '34px',
    },
  },
  subtitle: {
    marginTop: '20px',
    fontSize: '18px',
    lineHeight: 1.5,
    color: tokens.colorNeutralForeground2,
    maxWidth: '520px',
  },
  ctaRow: {
    display: 'flex',
    gap: '16px',
    marginTop: '32px',
    flexWrap: 'wrap',
  },
  ctaPrimary: {
    backgroundColor: brandAccent.amber,
    color: brandAccent.ink,
    fontWeight: 600,
    ':hover': { backgroundColor: brandAccent.amberDark, color: brandAccent.ink },
    ':hover:active': { backgroundColor: brandAccent.amberDark, color: brandAccent.ink },
  },
  heroCard: {
    borderRadius: '24px',
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    boxShadow: '0 18px 23px -5px rgba(234, 243, 255, 0.55), 0 7px 9px -5px rgba(234, 243, 255, 0.55)',
    padding: '24px',
    maxWidth: '480px',
    justifySelf: 'center',
  },
  bubbleAssistant: {
    borderRadius: '6px 18px 18px 18px',
    backgroundColor: tokens.colorNeutralBackground3,
    padding: '12px 16px',
    marginBottom: '10px',
    maxWidth: '85%',
  },
  bubbleUser: {
    borderRadius: '18px 6px 18px 18px',
    backgroundColor: tokens.colorBrandBackground,
    color: tokens.colorNeutralForegroundOnBrand,
    padding: '12px 16px',
    marginBottom: '10px',
    maxWidth: '85%',
    marginLeft: 'auto',
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(3, 1fr)',
    gap: '20px',
    marginTop: '16px',
    '@media (max-width: 900px)': {
      gridTemplateColumns: '1fr',
    },
  },
  featureCard: {
    backgroundColor: tokens.colorNeutralBackground2,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    borderRadius: '22px',
    padding: '24px',
  },
  iconTile: {
    width: '42px',
    height: '42px',
    borderRadius: '10px',
    backgroundColor: brandColorsIconTile,
    color: tokens.colorNeutralBackground1,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize: '20px',
    marginBottom: '16px',
  },
  section: {
    marginTop: '80px',
  },
  sectionTitle: {
    fontFamily: "'TRICK', 'Parkinsans', sans-serif",
  },
});

const features = [
  {
    icon: '📬',
    title: 'Post en agenda in één oogopslag',
    body: 'Vraag Buddy welke e-mails aandacht nodig hebben of wat er vandaag op de agenda staat — geen inbox meer doorspitten.',
  },
  {
    icon: '📄',
    title: 'Bestanden snel terugvinden',
    body: 'Buddy doorzoekt je eigen OneDrive-bestanden op naam of inhoud, zodat je nooit meer een document kwijt bent.',
  },
  {
    icon: '🔒',
    title: 'Alleen jouw eigen gegevens',
    body: 'Buddy leest alleen wat jij zelf mag zien, via je eigen Microsoft 365-login — nooit gegevens van collega’s.',
  },
];

export const LandingPage = () => {
  const styles = useStyles();

  return (
    <MarketingLayout>
      <section className={styles.hero}>
        <div>
          <span className={styles.eyebrow}>● Live verbonden met Microsoft 365</span>
          <Title1 className={styles.title} as="h1">
            Je digitale collega voor Microsoft 365
          </Title1>
          <Body1 as="p" className={styles.subtitle}>
            M365 Buddy beantwoordt vragen over je post, agenda en bestanden — rechtstreeks vanuit je eigen
            Microsoft 365-account, zonder dat je hoeft te schakelen tussen apps.
          </Body1>
          <div className={styles.ctaRow}>
            <Button as="a" href="/app" size="large" shape="circular" className={styles.ctaPrimary}>
              Open Buddy
            </Button>
            <Button as="a" href="#hoe-het-werkt" size="large" shape="circular" appearance="outline">
              Hoe het werkt
            </Button>
          </div>
        </div>

        <div className={styles.heroCard}>
          <div className={styles.bubbleUser}>
            <Text>Welke e-mails hebben vandaag aandacht nodig?</Text>
          </div>
          <div className={styles.bubbleAssistant}>
            <Text>
              Ik heb je inbox bekeken — 3 berichten lijken belangrijk, waaronder een vraag van Maurice over de
              planning. Wil je dat ik ze samenvat?
            </Text>
          </div>
        </div>
      </section>

      <section className={styles.section} id="hoe-het-werkt">
        <Title1 as="h2" className={styles.sectionTitle} style={{ fontSize: '32px' }}>Wat Buddy vandaag al kan</Title1>
        <div className={styles.grid}>
          {features.map((feature) => (
            <div key={feature.title} className={styles.featureCard}>
              <div className={styles.iconTile}>{feature.icon}</div>
              <Text weight="semibold" size={500} block style={{ marginBottom: '8px' }}>
                {feature.title}
              </Text>
              <Text style={{ color: tokens.colorNeutralForeground2 }}>{feature.body}</Text>
            </div>
          ))}
        </div>
      </section>

      <section className={styles.section}>
        <div className={styles.featureCard} style={{ textAlign: 'center', padding: '48px 24px' }}>
          <Title1 as="h2" className={styles.sectionTitle} style={{ fontSize: '28px', marginBottom: '12px' }}>
            Klaar om te starten?
          </Title1>
          <Body1 style={{ color: tokens.colorNeutralForeground2, display: 'block', marginBottom: '24px' }}>
            Log in met je eigen Microsoft 365-account — geen aparte registratie nodig.
          </Body1>
          <Button as="a" href="/app" size="large" shape="circular" className={styles.ctaPrimary}>
            Open Buddy
          </Button>
        </div>
      </section>
    </MarketingLayout>
  );
};
