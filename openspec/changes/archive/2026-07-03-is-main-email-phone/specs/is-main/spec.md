# Delta for IsMain on Email/Phone Entities

## ADDED Requirements

### Requirement: IsMain field on all base DTOs

6 base DTOs MUST expose `bool IsMain { get; init; }` defaulting to `false`.

| DTO | Addition |
|-----|----------|
| `EntityEmailDto`, `EntityPhoneDto` | `bool IsMain` |
| `CreateEntityEmailRequest`, `CreateEntityPhoneRequest` | `bool IsMain` |
| `UpdateEntityEmailRequest`, `UpdateEntityPhoneRequest` | `bool IsMain` |

#### Scenario: Read emits IsMain

- GIVEN a business with email `a@b.com` marked IsMain
- WHEN API returns `BusinessDto`
- THEN `Emails[0].IsMain` MUST be `true`

#### Scenario: Create auto-assigns first

- GIVEN 2 emails in create, neither marked IsMain
- WHEN service processes
- THEN first email saved with `IsMain = true`

### Requirement: Id field on Update request DTOs

`UpdateEntityEmailRequest` and `UpdateEntityPhoneRequest` MUST include `int? Id` for client-side diff matching.

#### Scenario: Update matches by Id

- GIVEN update with `[{Id: 1, Email: "a@b.com"}, {Id: null, Email: "new@c.com"}]`
- WHEN service syncs
- THEN email with `Id=1` updated, `new@c.com` added

### Requirement: PhoneType in CreateEntityPhoneRequest

`CreateEntityPhoneRequest` MUST include `PhoneType` (enum).

#### Scenario: Create specifies phone type

- GIVEN `PhoneType = PhoneType.Work` in create
- WHEN service creates
- THEN phone persisted with `PhoneType.Work`

### Requirement: Business rule — exactly one IsMain per type

Exactly one email and one phone per parent MUST have `IsMain = true`.

| Condition | Action |
|-----------|--------|
| None marked | First item auto-assigned IsMain |
| Multiple marked | First wins; others forced to `false` |
| Only one item | Always IsMain (overrides request) |
| Toggle to another | Previous unsets; new one becomes IsMain |

#### Scenario: Auto-assign when none marked

- GIVEN 2 emails, neither IsMain
- WHEN service validates
- THEN first gets `IsMain = true`, second `IsMain = false`

#### Scenario: Multiple IsMain corrected

- GIVEN 3 emails with `[true, true, false]`
- WHEN service validates
- THEN first marked keeps `true`; others reset

#### Scenario: Single overrides request

- GIVEN 1 phone with `IsMain = false`
- WHEN service creates
- THEN phone saved with `IsMain = true`

## MODIFIED Requirements

### Requirement: Parent request migration from List\<string\>

8 parent requests MUST replace `List<string>? Emails/Phones` with typed lists.

| Entity | Create Emails | Update Emails |
|--------|--------------|---------------|
| Business | `List<CreateEntityEmailRequest>?` | `List<UpdateEntityEmailRequest>?` |
| Branch, ORA, HPE | Same pattern | Same pattern |

Phones: same replacement using `CreateEntityPhoneRequest` / `UpdateEntityPhoneRequest`.
(Previously: `List<string>?` — no IsMain, no Id, no PhoneType)

#### Scenario: Create parent with typed items

- GIVEN `[{"Email": "a@b.com", "IsMain": true}]` in create request
- WHEN service creates
- THEN entity persisted with IsMain applied

#### Scenario: Update syncs via typed diff

- GIVEN typed list with mixed new/updated items
- WHEN service updates
- THEN items matched by Id are updated; new items added; omitted items removed

### Requirement: Service IsMain enforcement

4 services (Business, Branch, ORA, HPE) MUST apply IsMain business rules in Create/Update.
(Previously: raw-string processing, no IsMain; phones hardcoded to `PhoneType.Mobile`)

#### Scenario: Create enforces rules

- GIVEN create request with items
- WHEN service creates
- THEN IsMain rules applied before persistence

#### Scenario: Update preserves when list null

- GIVEN existing IsMain email and update omitting emails list
- WHEN `Emails` is null
- THEN existing emails are unchanged

### Requirement: Blazor UI IsMain selector

Create/edit forms MUST show an IsMain selector (radio/badge) per email/phone item, enforcing exactly one per type.
(Previously: raw string inputs, no IsMain UI)

#### Scenario: Form defaults first as IsMain

- GIVEN new form with 2 email inputs
- WHEN neither marked IsMain
- THEN first shows as IsMain indicator active

#### Scenario: Edit shows persisted IsMain

- GIVEN edit form loaded from API
- THEN each item shows its IsMain status
- AND toggling a new one auto-unsets the previous
