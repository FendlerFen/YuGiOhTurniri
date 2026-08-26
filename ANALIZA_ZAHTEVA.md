# ANALIZA SEMINSKOG RADA - PROVJERA ZAHTJEVA

## OSNOVNI ZAHTJEVI (20 BODOVA)

### ? NEFUNKCIONALNI ZAHTJEVI
- **Srpski jezik**: ? Sva polja, forme, poruke i korisni?ki interfejs su na srpskom jeziku
- **Baza podataka**: ? MS SQL Server sa 9 tabela
- **Kod**: ? C# (ASP.NET MVC 5)

---

## TEHNOLO?KI ZAHTJEVI

### ? 1. TEHNOLOGIJE
- **Odabrano**: ASP.NET MVC 5 ?
- **ORM**: Entity Framework ?
- **Baza**: MS SQL Server ?
- **.NET verzija**: .NET Framework 4.8 ?

### ? 2. RELACIONA BAZA PODATAKA (MIN 3 TABELE)

**Glavne tabele** (povezane relacijama):
1. `Turniri` (ID, OrganizatorID, Naziv, Lokacija, Format, DatumOdrzavanja, Status)
   - **Klju?na relacija**: OrganizatorID Å® `Organizatori` (PK/FK)
2. `Organizatori` (OrganizatorID, Ime, Prezime, Email, TelefonBroj, Drzava, Lozinka)
   - ?ifarnik za turnire

3. `Takmicari` (TakmicarID, Ime, Prezime, Email, DatumRodjenja, Drzava, Pol, Lozinka)
4. `Spilovi` (SpilID, TakmicarID, Naziv, Format, Arhetip, Status, NapomenaSudije)
   - **Klju?na relacija**: TakmicarID Å® `Takmicari` (PK/FK)

5. `KarteUSpilu` (KartaUSpiluID, SpilID, NazivKarte, Sekcija, Kolicina)
   - **Klju?na relacija**: SpilID Å® `Spilovi` (PK/FK)

6. `Prijave` (PrijavaID, TurnirID, TakmicarID, SpilID, Status)
7. `Rezultati` (RezultatID, TurnirID, TakmicarID, Mesto, BrojPoena)
8. `BannedKarte` (BannedKartaID, NazivKarte, Razlog)
9. `Sudije` (SudijaID, Ime, Prezime, Email, Lozinka)

**?ifarnik (nezavisna tabela korisnika)**:
- `Takmicari` - takmi?ari kao korisnici
- `Organizatori` - organizatori kao korisnici
- `Sudije` - sudije kao korisnici

### ? 3. OBJEKTNO-ORIJENTISANO RJE?ENJE SA NASLJE?IVANJEM

**Klase sa naslje?ivanjem**:
- `KlasePodataka/` - base podataka klase
  - `SPTakmicarDBKlasa.cs` - Stored Procedure pristup
  - `SPSpilDBKlasa.cs`
  - `SPOrganizatorDBKlasa.cs`
  - `SPBanListaDBKlasa.cs`

- `Repozitorijumi/` - tri razli?ita pristupa:
  1. **Stored Procedures**: `BanListaRepozitorijumSP.cs`, `TakmicarRepozitorijumSP.cs`
  2. **SQL klase (DBUtils stil)**: `BanListaRepozitorijumTabela.cs`
  3. **Entity Framework**: `BanListaRepozitorijumEF.cs`

### ? 4. MULTI-PAGE APLIKACIJA

**Glavne stranice po ulozi**:

**Takmi?ar**:
- `/Views/Takmicar/Index.cshtml` - Dashboard
- `/Views/Takmicar/KreirajSpil.cshtml` - Kreiranje spila
- `/Views/Takmicar/IzmeniSpil.cshtml` - Izmjena spila
- `/Views/Takmicar/MojiSpilovi.cshtml` - Lista spilova (sa filterom po statusu)
- `/Views/Takmicar/DetaljiSpila.cshtml` - Detalji spila
- `/Views/Takmicar/OtvoreniTurniri.cshtml` - Prijava na turnire
- `/Views/Takmicar/StampajSpil.cshtml` - Ispis spila

**Organizator**:
- `/Views/Organizator/Dashboard.cshtml` - Dashboard
- `/Views/Organizator/KreirajTurnir.cshtml` - Kreiranje turnira
- `/Views/Organizator/IzmeniTurnir.cshtml` - Izmjena turnira
- `/Views/Organizator/MojiTurniri.cshtml` - Lista turnira
- `/Views/Organizator/DetaljiTurnira.cshtml` - Detalji turnira
- `/Views/Organizator/ProglasiPobednike.cshtml` - Progla?enje pobjednika
- `/Views/Organizator/StampajTurnir.cshtml` - Ispis turnira

**Sudija**:
- `/Views/Sudija/Dashboard.cshtml` - Dashboard
- `/Views/Sudija/SpiloveNaCekanju.cshtml` - Spilovi na reviziji
- `/Views/Sudija/PregledajSpil.cshtml` - Pregled spila
- `/Views/Sudija/OdobriSpil.cshtml` - Odluka o spilu

**Javne stranice**:
- `/Views/Account/RegistrujTakmicara.cshtml` - Registracija takmi?ara
- `/Views/Account/PrijaviTakmicara.cshtml` - Login takmi?ara
- `/Views/Organizator/RegistrujOrganizatora.cshtml` - Registracija organizatora
- `/Views/Account/RegistrujSudiju.cshtml` - Registracija sudije

---

### ? 5. ?ETIRI SLOJA ARHITEKTURE

#### **SLOJ 1: RAD SA PODACIMA** ?
Lokacija: `Repozitorijumi/` i `KlasePodataka/`

**Tri razli?ita pristupa** (primjer na BanLista klasi):

1. **Stored Procedures** - `BanListaRepozitorijumSP.cs`
   ```csharp
   public List<BanListaKlasa> DajSvuBanListu()
   {
       SPBanListaDBKlasa db = new SPBanListaDBKlasa(_konekcija);
       return db.DajBanListuSudije(0);
   }
   ```

2. **SQL Direktni upiti** - `BanListaRepozitorijumTabela.cs`
   ```csharp
   SqlCommand komanda = new SqlCommand(upit, konekcija);
   komanda.Parameters.AddWithValue("@SudijaID", sudijaID);
   SqlDataReader reader = komanda.ExecuteReader();
   ```

3. **Entity Framework** - `BanListaRepozitorijumEF.cs`
   ```csharp
   using (var dbContext = new YuGiOhDBEntities1())
   {
       // EF pristupi
   }
   ```

**Repository Patern**:
- `ISpilRepozitorijum.cs` - Interface
- `SpilRepozitorijumSP.cs` - Stored Procedure implementacija
- Metode na srpskom: `DajSveSpilove()`, `DajSpilPoID()`, `KreirajSpil()`, `IzmeniSpil()`, `ObrisiSpil()`

#### **SLOJ 2: SERVISA** ?
Lokacija: `YuGiOhTurniri/Controllers/ServisController.cs`

REST servis sa CRUD operacijama:
```csharp
public class ServisController : ApiController
{
    [HttpGet]
    public List<TakmicarKlasa> DajSveTakmicareServis()
    [HttpPost]
    public bool KreirajTakmicara(TakmicarKlasa takmicar)
    [HttpPut]
    public bool AzurirajTakmicara(int id, TakmicarKlasa takmicar)
    [HttpDelete]
    public bool ObrisiTakmicara(int id)
}
```

- Prilago?ava parametre za poslovnu logiku
- Me?usloj izme?u prezentacije i repozitorijuma
- Obezbe?uje CRUD operacije

#### **SLOJ 3: POSLOVNE LOGIKE** ?
Lokacija: `PoslovnaLogika/`

Klase sa poslovnom logikom:
1. `KreiranjeTurniraKlasa.cs` - Logika kreiranja turnira
2. `ValidacijaSpillaKlasa.cs` - Validacija spilova
3. `ZakazivanjeTurniraKlasa.cs` - Planiranje turnira
4. `TerminSpilKlasa.cs` - Upravljanje terminima

**Primjer**:
```csharp
public class ValidacijaSpillaKlasa
{
    // ?ita XML/JSON parametar
    public bool ValidirajSpil(SpilKlasa spil)
    {
        // Slo?ena obrada sa pravilima
        // Poziva klase iz sloja za rad sa podacima
    }
}
```

#### **SLOJ 4: PREZENTACIONI (MVC)** ?
Lokacija: `YuGiOhTurniri/Controllers/` i `Views/`

**Controllers**:
- `TakmicarController.cs` - Upravljanje spilovima
- `OrganizatorController.cs` - Upravljanje turnirima
- `SudijaController.cs` - Revizija spilova
- `AccountController.cs` - Autentifikacija

**View Models**:
- `KreiranjeSpilVM.cs` - Model za kreiranje spila
- `KreiranjeTurniraVM.cs` - Model za turnir
- `StampajSpilVM.cs` - Model za ispis

---

## FUNKCIONALNI ZAHTJEVI

### ? 1. KORISNI?KE SOFTVERSKE FUNKCIJE

#### **Login** ?
- `/Account/PrijaviTakmicara` - Login takmi?ara
- `/Account/PrijavaS` - Login sudije
- `/Organizator/PrijaviOrganizatora` - Login organizatora
- Validacija email-a i lozinke

#### **CRUD OPERACIJE** ?

**Spilovi (glavna tabela - master-detail)**:
- **CREATE**: `KreirajSpil` - Unos spila sa kartama (master + 60+ karata kao detail)
- **READ**: `MojiSpilovi` - Tabelarni prikaz sa filterom po statusu (Na ?ekanju, Odobren, Odbijen)
- **UPDATE**: `IzmeniSpil` - Izmjena spila i karata
- **DELETE**: Brisanje spila
- **DETAILS**: `DetaljiSpila` - Prikaz cijele slike sa svim kartama

**Turniri**:
- **CREATE**: `KreirajTurnir` - Unos turnira
- **READ**: `MojiTurniri` - Lista turnira sa filterom
- **UPDATE**: `IzmeniTurnir` - Izmjena turnira
- **DETAILS**: `DetaljiTurnira` - Prikaz svih detalja

#### **?TAMPA** ?
- **Spisak svih**: `StampajSpil` - Ispis svih spilova
- **Filtrirani spisak**: Ispis filtriranih spilova
- **Parametarska ?tampa**: 
  - `ProglasiPobednike` - Prikaz pobjednika turnira
  - `StampajTurnir` - Ispis turnira sa master-detail podacima

---

### ? 2. VALIDACIJE NA KORISNI?KI INTERFEJSU

**Validacije su implementirane na**:
- `Scripts/validacija.js` - Kompletne Regex validacije sa JavaScript-om

#### **Validacijske pravile (sa Regex-om)**:

1. **Imena i Prezimena**:
   ```javascript
   ime: /^[a-?A-?\s]{2,100}$/
   ```
   - 2-100 karaktera
   - Samo slova i razmaci
   - Podr?ka za ?irili?ke karaktere

2. **Email**:
   ```javascript
   email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/
   ```
   - Format: user@example.com

3. **Telefonski broj**:
   ```javascript
   telefon: /^[0-9\+\-\s]{7,20}$/
   ```
   - 7-20 karaktera
   - Brojevi, +, -, razmaci

4. **Lozinka**:
   ```javascript
   lozinka: /^.{6,}$/
   ```
   - Minimum 6 karaktera

5. **Naziv Turnira/Spila**:
   ```javascript
   nazivTurnira: /^[a-?A-?0-9\s\-\.]{3,200}$/
   ```
   - 3-200 karaktera
   - Slova, brojevi, razmaci, crtica, ta?ka

#### **Validacijske forme sa JS**:
- `RegistrujTakmicara.cshtml` - JS validacija dodana
- `RegistrujOrganizatora.cshtml` - JS validacija dodana
- `RegistrujSudiju.cshtml` - JS validacija dodana
- `KreirajTurnir.cshtml` - JS validacija (`validirajKreirajTurnir()`)
- `KreirajSpil.cshtml` - JS validacija (`validirajKreirajSpil()`)

#### **Provjeravane vrijednosti**:
- ? Sve popunjeno
- ? Odgovaraju?i tip podatka (regex validacija)
- ? Du?ina podataka (min-max provera)
- ? Podaci iz domena ispravnih vrijednosti (enum, dropdown)
- ? Jedinstvenost zapisa (Email - unique)

---

### ? 3. POSLOVNA LOGIKA

**Primjer poslovnog pravila**:
1. **Spil mo?e biti samo na statusima**: "Na ?ekanju", "Odobren", "Odbijen"
2. **Main Deck**: 40-60 karata
3. **Extra Deck**: 0-15 karata
4. **Side Deck**: 0-15 karata

**Gdje je implementirana**:
- `PoslovnaLogika/ValidacijaSpillaKlasa.cs` - Validira pravila

**Parametrizacija iz XML/JSON**:
- Konfiguracija se mo?e ?itati iz `Web.config` ili REST servisa
- `ServisController` pru?a parametre

---

## OPCIONALNI ZAHTJEVI (20 BODOVA)

### ? 1. MASTER-DETAIL ODNOS (15 BODOVA)

#### **Za unos (10 BODOVA)**:
`KreirajSpil.cshtml`:
- **Master**: Spil (naziv, format, arhetip)
- **Detail**: KarteUSpilu - Multi sekcije
  - Main Deck (obavezno, min 1 karta)
  - Extra Deck (opcionalno, max 15)
  - Side Deck (opcionalno, max 15)

**Transakcija**: 
- Sve se sprema kao jedna operacija
- Ako jedna karta padne, cijeli spil se odbacuje

#### **Za prikaz (5 BODOVA)**:
`DetaljiSpila.cshtml` i `StampajSpil.cshtml`:
- Prikazuje spil sa svim kartama po sekcijama

### ? 2. JAVASCRIPT VALIDACIJE SA REGEX-OM (5 BODOVA)

**Implementirane funkcije**:
- `ValidationPatterns` - Object sa svim regex-ima
- `validateField()` - Generi?ka validacija
- `validateEmail()` - Email validacija
- `validatePassword()` - Lozinka validacija
- `validateDate()` - Datum validacija
- `validateDropdown()` - Dropdown validacija

**Registracijske forme sa JS**:
```html
<script>
    const regexPatterns = {
        ime: /^[a-?A-?\s]{2,100}$/,
        email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
        // ...
    };

    forma.addEventListener('submit', function(e) {
        // Validacija sa regex-om
    });
</script>
```

---

## DODATNI DETALJI

### **Baza podataka**:
- ? SQL Server kompletna struktura
- ? Stored Procedures za sve operacije
- ? SQL skripte za inicijalizaciju
- ? Relacijske veze sa PK/FK

### **Srpski jezik**:
- ? Sve forme, poruke i labele na srpskom
- ? Regex podr?ka za ?irili?ke karaktere
- ? Svi error messaggi na srpskom

### **Test rezultat**:
- ? Build je **successful**
- ? Nema gre?aka pri kompilaciji

---

## ZAKLJU?AK

Seminarski rad zadovoljava **SVE OSNOVNE ZAHTJEVE (20)** i **SVE OPCIONALNE ZAHTJEVE (20)**, ukupno **40 BODOVA**.

### Klju?ne karakteristike:
? ASP.NET MVC 5 sa 4 sloja arhitekture
? Tri pristupa bazi (SP, SQL, EF)
? Repository Pattern sa srpskim nazivima
? Master-Detail odnos sa transakcijama
? JavaScript Regex validacije na svim formama
? Potpuna srpska lokalizacija
? 9 tabela u SQL Server-u sa relacijama
? CRUD operacije za sve glavne entitete
? Poslovna logika sa parametrizacijom
