import { describe, it, expect, vi, beforeEach } from 'vitest';

describe('authConfig', () => {
  beforeEach(() => {
    vi.resetModules();
  });

  it('uses SPA client ID for scopes when backend client ID not set', async () => {
    vi.stubEnv('VITE_ENTRA_SPA_CLIENT_ID', 'spa-client-id');
    vi.stubEnv('VITE_ENTRA_TENANT_ID', 'tenant-id');
    vi.stubEnv('VITE_ENTRA_BACKEND_CLIENT_ID', '');
    const { loginRequest } = await import('../../config/authConfig');
    expect(loginRequest.scopes[0]).toBe('api://spa-client-id/Chat.ReadWrite');
  });

  it('uses backend client ID for scopes when set', async () => {
    vi.stubEnv('VITE_ENTRA_SPA_CLIENT_ID', 'spa-client-id');
    vi.stubEnv('VITE_ENTRA_TENANT_ID', 'tenant-id');
    vi.stubEnv('VITE_ENTRA_BACKEND_CLIENT_ID', 'backend-client-id');
    const { loginRequest } = await import('../../config/authConfig');
    expect(loginRequest.scopes[0]).toBe('api://backend-client-id/Chat.ReadWrite');
  });

  it('uses the multi-tenant "organizations" authority, not a tenant-specific one', async () => {
    // The app registrations are AzureADMultipleOrgs (infra/entra-app.bicep) — a tenant-specific
    // authority here would reject every user outside our own tenant.
    vi.stubEnv('VITE_ENTRA_SPA_CLIENT_ID', 'spa-client-id');
    const { msalConfig } = await import('../../config/authConfig');
    expect(msalConfig.auth.authority).toBe('https://login.microsoftonline.com/organizations');
  });

  it('does not require VITE_ENTRA_TENANT_ID (multi-tenant authority does not depend on it)', async () => {
    vi.stubEnv('VITE_ENTRA_SPA_CLIENT_ID', 'spa-client-id');
    vi.stubEnv('VITE_ENTRA_TENANT_ID', '');
    await expect(import('../../config/authConfig')).resolves.toBeDefined();
  });

  it('redirects to /app regardless of origin', async () => {
    vi.stubEnv('VITE_ENTRA_SPA_CLIENT_ID', 'spa-client-id');
    const { msalConfig } = await import('../../config/authConfig');
    expect(msalConfig.auth.redirectUri).toBe(`${window.location.origin}/app`);
  });
});
