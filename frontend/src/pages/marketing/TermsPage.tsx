import { LegalPage } from './LegalPage';

// Content adapted from m365buddy.nl/algemene-voorwaarden.html.
export const TermsPage = () => (
  <LegalPage
    title="Algemene voorwaarden"
    intro="Deze voorwaarden leggen vast wat je van M365 Buddy mag verwachten als digitale Microsoft 365-collega, en welke afspraken gelden zodra je organisatie de dienst gebruikt."
    sections={[
      {
        heading: '1. De dienst',
        body: (
          <>
            <p>
              M365 Buddy is een AI-assistent die medewerkers binnen Microsoft 365 helpt met vragen over
              applicaties, processen en de eigen Outlook, agenda en bestanden. De dienst wordt geleverd door
              Skills4-IT ("wij", "M365 Buddy") aan organisaties ("klant", "u") die zich hebben aangemeld via
              een offerte, proefperiode of abonnement.
            </p>
            <p>
              Door M365 Buddy te activeren binnen uw Microsoft 365-omgeving gaat u akkoord met deze algemene
              voorwaarden, aanvullend op eventuele afzonderlijke verwerkersovereenkomst (DPA).
            </p>
          </>
        ),
      },
      {
        heading: '2. Toegang & app-registratie',
        body: 'M365 Buddy koppelt met uw tenant via een officiële Microsoft Entra ID app-registratie en het OAuth 2.0-protocol (on-behalf-of, delegated). Toegang is beperkt tot wat expliciet is geconsenteerd door uw beheerder en kan op elk moment worden ingetrokken.',
      },
      {
        heading: '3. Gebruik van M365 Buddy',
        body: 'U mag M365 Buddy gebruiken voor interne, zakelijke doeleinden binnen uw organisatie. Het is niet toegestaan om de dienst te gebruiken voor onrechtmatige doeleinden, om geautomatiseerd grote hoeveelheden data te onttrekken, of om de dienst te reverse-engineeren.',
      },
      {
        heading: '4. Gegevens & verwerking',
        body: (
          <>
            M365 Buddy verwerkt alleen de gegevens die nodig zijn om een vraag te beantwoorden, real-time en
            zonder onnodige opslag van gevoelige bedrijfsinformatie. Voor een volledig overzicht van welke
            gegevens worden verwerkt, waar en waarom, verwijzen we naar onze{' '}
            <a href="/privacy">privacyverklaring</a>.
          </>
        ),
      },
      {
        heading: '5. Beschikbaarheid & ondersteuning',
        body: (
          <>
            Wij spannen ons in voor een hoge beschikbaarheid van de dienst. Gepland onderhoud wordt vooraf
            gecommuniceerd. Supportvragen kunnen worden gemeld via{' '}
            <a href="mailto:support@m365buddy.nl">support@m365buddy.nl</a>; reactietijden zijn afhankelijk van
            het afgesproken serviceniveau.
          </>
        ),
      },
      {
        heading: '6. Abonnement & opzegging',
        body: 'Abonnementsvoorwaarden, looptijd en opzegtermijn worden vastgelegd in uw offerte of overeenkomst. Bij beëindiging wordt toegang tot de Microsoft Graph-gegevens van uw tenant per direct stopgezet en verwijderen wij eventueel opgeslagen configuratiegegevens binnen 30 dagen, tenzij wettelijk anders vereist.',
      },
      {
        heading: '7. Aansprakelijkheid',
        body: 'M365 Buddy is een hulpmiddel dat antwoorden genereert op basis van beschikbare informatie en AI. Antwoorden kunnen onjuistheden bevatten; gebruik bij kritieke beslissingen altijd uw eigen beoordelingsvermogen. Onze aansprakelijkheid is beperkt tot hetgeen is overeengekomen in de toepasselijke overeenkomst.',
      },
      {
        heading: '8. Wijzigingen & contact',
        body: (
          <>
            Wij kunnen deze voorwaarden periodiek actualiseren om deze in lijn te houden met wet- en
            regelgeving of productontwikkeling. Wezenlijke wijzigingen communiceren we vooraf. Vragen over
            deze voorwaarden? Neem contact op via <a href="mailto:support@m365buddy.nl">support@m365buddy.nl</a>.
          </>
        ),
      },
    ]}
  />
);
