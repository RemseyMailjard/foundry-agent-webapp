import type { ReactNode } from 'react';
import { makeStyles, tokens, Title1, Text } from '@fluentui/react-components';
import { MarketingLayout } from './MarketingLayout';

const useStyles = makeStyles({
  intro: {
    color: tokens.colorNeutralForeground2,
    fontSize: '18px',
    lineHeight: '1.5',
    maxWidth: '720px',
    marginTop: '12px',
  },
  content: {
    maxWidth: '720px',
    marginTop: '40px',
    display: 'flex',
    flexDirection: 'column',
    gap: '32px',
  },
});

export interface LegalSection {
  heading: string;
  body: ReactNode;
}

export const LegalPage = ({
  title,
  intro,
  sections,
}: {
  title: string;
  intro: string;
  sections: LegalSection[];
}) => {
  const styles = useStyles();

  return (
    <MarketingLayout>
      <Title1>{title}</Title1>
      <Text as="p" className={styles.intro}>{intro}</Text>

      <div className={styles.content}>
        {sections.map((section) => (
          <section key={section.heading}>
            <Text as="h2" weight="semibold" size={500} block style={{ marginBottom: '8px' }}>
              {section.heading}
            </Text>
            <div style={{ color: tokens.colorNeutralForeground2, lineHeight: '1.6' }}>
              {section.body}
            </div>
          </section>
        ))}
      </div>
    </MarketingLayout>
  );
};
