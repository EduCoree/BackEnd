# Branch: feature/certificates-controller

## Scope

Implements CertificatesController with GET /my (Student) and
GET /{certificateUuid} + GET /{certificateUuid}/view (Public).
Adds ICertificateService, CertificateService, and CertificateResponse
DTO if missing. Certificate auto-insert already handled by ProgressService.

## New Files

- EduCore.Shared/DTOs/Progress/CertificateResponse.cs
- EduCore.Services Abstraction/ICertificateService.cs
- EduCore.Services/CertificateService.cs
- EduCore.Presentation/Controllers/CertificatesController.cs

## Modified Files

- Program.cs — one DI line appended only

## API Endpoints Consumed

- GET /api/certificates/my          [Student]
- GET /api/certificates/{uuid}      [Public]
- GET /api/certificates/{uuid}/view [Public]

## Dependencies

feature/progress-models-services must be merged first.
