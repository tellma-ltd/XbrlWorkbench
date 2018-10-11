

__________                                ___________           .__              ____  _______________________.____     
\______   \_____    ____ _____    ____    \__    ___/___   ____ |  |   ______    \   \/  /\______   \______   \    |    
 |    |  _/\__  \  /    \\__  \  /    \     |    | /  _ \ /  _ \|  |  /  ___/     \     /  |    |  _/|       _/    |    
 |    |   \ / __ \|   |  \/ __ \|   |  \    |    |(  <_> |  <_> )  |__\___ \      /     \  |    |   \|    |   \    |___ 
 |______  /(____  /___|  (____  /___|  / /\ |____| \____/ \____/|____/____  > /\ /___/\  \ |______  /|____|_  /_______ \
        \/      \/     \/     \/     \/  \/                               \/  \/       \_/        \/        \/        \/

"It costs a lot of money to look this cheap." - Dolly Parton


Import
======

Add a Taxonomy from a ZIP Archive
---------------------------------
var zipArchive = ZipFile.Open(archiveFileName, ZipArchiveMode.Read);
var fileReader = new ZipArchiveReader(zipArchive);
var taxonomyFileSet = new TaxonomyFileSet(fileReader);
instance.Dts.AddTaxonomy(entryPointUri, taxonomyFileSet);

Add an Extension Taxonomy
-------------------------
var extension = new TaxonomyExtension(targetNamespacePrefix, targetNamespace);
// Add extension concepts and extension members together with their label(s) and location(s)
instance.Dts.AddTaxonomy(entryPointUri, extension);


Export
======
Writer classes allow exporting XBRL instances and taxonomies in various formats. 
This NuGet package contains the following writers:

InlineXbrlWriter        exports an instance as an iXBRL XHTML file for display in a browser
InlineXbrlFilingWriter  exports an instance as a zip archive containing an iXBRL XHTML file together with all extension taxonomy schemata and linkbases
WordWriter              exports an instance as a DOCX document for display in Microsoft Word
XbrlWriter              exports an instance as an XBRL XML file
ZipArchiveWriter        exports a taxonomy as a zip archive containing all schemata and linkbases


Export to iXBRL XHTML file
--------------------------
var writer = new InlineXbrlWriter(template, writerSettings);
instance.Save(writer);
// result is available in writer.Document

Export to Filing Zip Archive
----------------------------
var writer = new InlineXbrlFilingWriter(template, writerSettings);
instance.Save(writer);
// result is available in writer.ZipArchiveBytes

Export to Word file
-------------------
var writer = new WordWriter(template, writerSettings);
instance.Save(writer);
// result is available in writer.DocumentBytes
