﻿using System.Reflection;
using MelonLoader;
using PoesQOL;

[assembly: AssemblyTitle(ModInfo.Name)]
[assembly: AssemblyDescription(ModInfo.Description)]
[assembly: AssemblyCompany(ModInfo.Company)]
[assembly: AssemblyProduct(ModInfo.Name)]
[assembly: AssemblyCopyright("Copyright ©  2024")]
[assembly: AssemblyTrademark(ModInfo.Company)]
[assembly: AssemblyVersion(ModInfo.Version)]
[assembly: AssemblyFileVersion(ModInfo.Version)]
[assembly: MelonInfo(typeof(PoesQOL.PoesQOL), ModInfo.Name, ModInfo.Version, ModInfo.Author, ModInfo.DownloadLink)]

[assembly: MelonGame("poncle", "Vampire Survivors")]