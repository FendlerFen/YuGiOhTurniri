# Zatvori Visual Studio procese
Get-Process devenv -ErrorAction SilentlyContinue | Stop-Process -Force

Start-Sleep -Seconds 2

# Pro?itaj .csproj fajl
$csprojPath = "C:\Visual cancer\YuGiOhTurniri\Servisa\Servisa.csproj"
[xml]$csproj = Get-Content $csprojPath

# Kreiraj novi ItemGroup za ProjectReference
$xmlns = "http://schemas.microsoft.com/developer/msbuild/2003"
$projectItemGroup = $csproj.CreateElement("ItemGroup")
$projectItemGroup.SetAttribute("xmlns", $xmlns)

# Dodaj KlasePodataka referencu
$klasePodatakaRef = $csproj.CreateElement("ProjectReference", $xmlns)
$klasePodatakaRef.SetAttribute("Include", "..\KlasePodataka\KlasePodataka.csproj")

$projectElem1 = $csproj.CreateElement("Project", $xmlns)
$projectElem1.InnerText = "{22B7494C-42EF-42A5-87FC-DF847F07AE48}"
$klasePodatakaRef.AppendChild($projectElem1)

$nameElem1 = $csproj.CreateElement("Name", $xmlns)
$nameElem1.InnerText = "KlasePodataka"
$klasePodatakaRef.AppendChild($nameElem1)

$projectItemGroup.AppendChild($klasePodatakaRef)

# Dodaj Repozitorijumi referencu
$repozitorijumiRef = $csproj.CreateElement("ProjectReference", $xmlns)
$repozitorijumiRef.SetAttribute("Include", "..\Repozitorijumi\Repozitorijumi.csproj")

$projectElem2 = $csproj.CreateElement("Project", $xmlns)
$projectElem2.InnerText = "{6DF13C90-5CB6-4244-92AD-9DA916CFC2B3}"
$repozitorijumiRef.AppendChild($projectElem2)

$nameElem2 = $csproj.CreateElement("Name", $xmlns)
$nameElem2.InnerText = "Repozitorijumi"
$repozitorijumiRef.AppendChild($nameElem2)

$projectItemGroup.AppendChild($repozitorijumiRef)

# Prona?i gde da ubaci novi ItemGroup
$lastItemGroup = $csproj.DocumentElement.SelectNodes("//ItemGroup")[-1]
$csproj.DocumentElement.InsertAfter($projectItemGroup, $lastItemGroup)

# Sa?uva fajl
$csproj.Save($csprojPath)

Write-Host "ProjectReferences dodani u Servisa.csproj"
