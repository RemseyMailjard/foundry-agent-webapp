import { LegalPage } from './LegalPage';

// Content adapted from m365buddy.nl/privacy.html — updated to describe this app's actual
// architecture (Azure AI Foundry + Microsoft Graph OBO) instead of the old demo's backend.
export const PrivacyPage = () => (
  <LegalPage
    title="Privacyverklaring"
    intro="M365 Buddy is gebouwd om medewerkers te helpen zonder onnodig gegevens te verzamelen. Hieronder leggen we precies uit welke Microsoft 365-informatie we inzien, waarom, en hoe u de regie behoudt."
    sections={[
      {
        heading: '1. Wie zijn wij',
        body: 'M365 Buddy is een dienst van Skills4-IT. Wanneer uw organisatie M365 Buddy activeert, treedt Skills4-IT op als verwerker van de Microsoft 365-gegevens die nodig zijn om medewerkersvragen te beantwoorden; uw organisatie blijft verwerkingsverantwoordelijke.',
      },
      {
        heading: '2. Welke gegevens verwerken we',
        body: 'M365 Buddy leest alleen gegevens via de officiële Microsoft Graph API, op basis van de permissies die uw beheerder heeft goedgekeurd (delegated, alleen-lezen): uw basisprofiel, recente e-mail (onderwerp, afzender, ontvangsttijd, leesstatus en een korte preview — nooit de volledige inhoud), agenda-afspraken en bestanden in OneDrive. Buddy leest nooit meer dan strikt nodig is om uw vraag te beantwoorden.',
      },
      {
        heading: '3. Grondslag & doel',
        body: 'De verwerking is gebaseerd op de overeenkomst tussen uw organisatie en Skills4-IT (uitvoering van de dienst) en op het gerechtvaardigd belang om de dienst te verbeteren. Gegevens worden uitsluitend gebruikt om de gevraagde ondersteuning te bieden — niet voor marketingdoeleinden richting individuele medewerkers.',
      },
      {
        heading: '4. Toestemming via Microsoft Entra ID',
        body: 'Toegang verloopt via een officiële app-registratie in Microsoft Entra ID, met OAuth 2.0 on-behalf-of. Uw IT-beheerder verleent bewust admin consent voor specifieke Microsoft Graph-permissies. Dit consent kan op elk moment worden ingetrokken via het Microsoft 365 admin center.',
      },
      {
        heading: '5. Bewaartermijn',
        body: 'Microsoft 365 blijft de bron van waarheid: Buddy haalt gegevens op wanneer nodig in plaats van ze permanent te spiegelen. Alleen conversatiemetadata en de eigen antwoorden van Buddy worden bewaard, met een standaard bewaartermijn van maximaal 90 dagen tenzij anders overeengekomen. Bij beëindiging van de dienst worden gekoppelde gegevens binnen 30 dagen verwijderd.',
      },
      {
        heading: '6. Delen met derden',
        body: 'We delen gegevens niet met derden voor commerciële doeleinden. Voor de technische levering van de dienst maken we gebruik van zorgvuldig geselecteerde subverwerkers (Microsoft Azure-infrastructuur binnen de EER), met wie passende verwerkersovereenkomsten zijn gesloten.',
      },
      {
        heading: '7. Beveiliging',
        body: 'We passen technische en organisatorische maatregelen toe, waaronder versleuteling van gegevens onderweg en in rust, rolgebaseerde toegang via Microsoft Entra ID, en logging van API-toegang via Microsoft Graph. Toegang tot productiesystemen is beperkt tot geautoriseerd personeel.',
      },
      {
        heading: '8. Uw rechten',
        body: (
          <>
            Als medewerker van een organisatie die M365 Buddy gebruikt, kunt u bij uw eigen IT-beheerder of via{' '}
            <a href="mailto:privacy@m365buddy.nl">privacy@m365buddy.nl</a> terecht voor inzage, correctie of
            verwijdering van gegevens, conform de AVG.
          </>
        ),
      },
      {
        heading: '9. Contact & klachten',
        body: (
          <>
            Vragen over deze privacyverklaring kunt u sturen naar{' '}
            <a href="mailto:privacy@m365buddy.nl">privacy@m365buddy.nl</a>. U heeft ook het recht een klacht in
            te dienen bij de Autoriteit Persoonsgegevens.
          </>
        ),
      },
    ]}
  />
);
