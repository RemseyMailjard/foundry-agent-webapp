import { Routes, Route } from 'react-router-dom';
import ChatApp from './pages/ChatApp';
import { LandingPage } from './pages/marketing/LandingPage';
import { PrivacyPage } from './pages/marketing/PrivacyPage';
import { TermsPage } from './pages/marketing/TermsPage';

/**
 * Public marketing/legal pages ("/", "/privacy", "/voorwaarden") render with no auth
 * requirement. The signed-in chat product lives entirely behind "/app" (see ChatApp.tsx),
 * which is the only route that forces an Entra ID sign-in.
 */
function App() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/privacy" element={<PrivacyPage />} />
      <Route path="/voorwaarden" element={<TermsPage />} />
      <Route path="/app" element={<ChatApp />} />
    </Routes>
  );
}

export default App;
