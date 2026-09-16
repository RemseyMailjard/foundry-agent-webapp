import type { ReactNode } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { makeStyles, tokens, Button, Text } from '@fluentui/react-components';
import { brandAccent } from '../../config/themes';

const useStyles = makeStyles({
  page: {
    minHeight: '100vh',
    backgroundColor: tokens.colorNeutralBackground1,
    color: tokens.colorNeutralForeground1,
  },
  header: {
    position: 'sticky',
    top: 0,
    zIndex: 10,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: '16px',
    maxWidth: '1120px',
    margin: '16px auto 0',
    padding: '0 20px',
    height: '70px',
    borderRadius: '24px',
    backgroundColor: tokens.colorNeutralBackground1,
    boxShadow: tokens.shadow8,
  },
  logo: {
    display: 'flex',
    alignItems: 'center',
    gap: '10px',
    textDecoration: 'none',
    color: 'inherit',
  },
  logoImg: {
    height: '28px',
  },
  nav: {
    display: 'flex',
    alignItems: 'center',
    gap: '24px',
  },
  navLink: {
    color: tokens.colorNeutralForeground2,
    textDecoration: 'none',
    fontWeight: 600,
    fontSize: '14px',
    ':hover': {
      color: tokens.colorBrandForeground1,
    },
  },
  ctaButton: {
    backgroundColor: brandAccent.amber,
    color: brandAccent.ink,
    fontWeight: 600,
    ':hover': {
      backgroundColor: brandAccent.amberDark,
      color: brandAccent.ink,
    },
    ':hover:active': {
      backgroundColor: brandAccent.amberDark,
      color: brandAccent.ink,
    },
  },
  main: {
    maxWidth: '1120px',
    margin: '0 auto',
    padding: '56px 20px',
  },
  footer: {
    borderTop: `1px solid ${tokens.colorNeutralStroke2}`,
    marginTop: '80px',
    padding: '32px 20px',
    display: 'flex',
    flexWrap: 'wrap',
    gap: '16px',
    alignItems: 'center',
    justifyContent: 'space-between',
    maxWidth: '1120px',
    marginLeft: 'auto',
    marginRight: 'auto',
  },
  footerLinks: {
    display: 'flex',
    gap: '20px',
  },
  footerLink: {
    color: tokens.colorNeutralForeground3,
    textDecoration: 'none',
    fontSize: '13px',
    ':hover': {
      color: tokens.colorNeutralForeground1,
    },
  },
});

export const MarketingLayout = ({ children }: { children: ReactNode }) => {
  const styles = useStyles();

  return (
    <div className={`${styles.page} marketing-page`}>
      <header className={styles.header}>
        <RouterLink to="/" className={styles.logo}>
          <img src="/brand/m365-buddy-logo-blue.svg" alt="M365 Buddy" className={styles.logoImg} />
        </RouterLink>
        <nav className={styles.nav}>
          <RouterLink to="/privacy" className={styles.navLink}>Privacy</RouterLink>
          <RouterLink to="/voorwaarden" className={styles.navLink}>Voorwaarden</RouterLink>
          <Button as="a" href="/app" className={styles.ctaButton} shape="circular">
            Open Buddy
          </Button>
        </nav>
      </header>

      <main className={styles.main}>{children}</main>

      <footer className={styles.footer}>
        <Text size={200} style={{ color: tokens.colorNeutralForeground3 }}>
          © {new Date().getFullYear()} M365 Buddy — een dienst van Skills4-IT
        </Text>
        <div className={styles.footerLinks}>
          <RouterLink to="/privacy" className={styles.footerLink}>Privacyverklaring</RouterLink>
          <RouterLink to="/voorwaarden" className={styles.footerLink}>Algemene voorwaarden</RouterLink>
        </div>
      </footer>
    </div>
  );
};
