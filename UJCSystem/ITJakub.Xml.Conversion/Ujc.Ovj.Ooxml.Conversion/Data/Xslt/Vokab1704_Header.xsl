<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xd tei" version="2.0">
    
    <xsl:include href="TEI_Common.xsl"/>
    
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Dec 5, 2015</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
    <xsl:preserve-space elements="text"/>
    <xsl:variable name="vychozi-jazyk" select="'cs'"/>
    
    
    
    <xsl:template match="/">
        <xsl:text xml:space="preserve">
</xsl:text>
        <xsl:comment> Vokab1704_Hlavicka </xsl:comment>
        <xsl:text xml:space="preserve">
</xsl:text>
        <TEI xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:call-template name="insert-header" />
            <text xmlns="http://www.tei-c.org/ns/1.0">
                <xsl:copy-of select="tei:body"/>
            </text>
        </TEI>
    </xsl:template>
    
    <xsl:template name="insert-header">
        <teiHeader xmlns="http://www.tei-c.org/ns/1.0" xml:id="Vokab1704" n="Vokab1704">
            <fileDesc n="{{9F14A489-FFDC-4144-A077-7BEB966E0E2A}}">
                <titleStmt>
                    <title>Vokabulář latinský, český i německý</title>
                </titleStmt>
                <editionStmt>
                    
                    <edition>digitální slovník</edition>
                    <respStmt><resp>editor</resp><name>Berger, Tilman</name></respStmt>
                    <respStmt><resp>editor</resp><name>Kuderová, Pavlína</name></respStmt>
                    <respStmt><resp>kódování TEI</resp><name>Lehečka, Boris</name></respStmt>
                </editionStmt>
                <publicationStmt>
                    <publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email></publisher>
                    <pubPlace>Praha</pubPlace>
                    <date>2015</date>
                    <availability status="restricted">
                        <p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
                    </availability>
                </publicationStmt>
                <sourceDesc n="bibl">
                    <p>
                        <bibl type="source"><author><forename>Jan Václav</forename> <surname>Pohl</surname></author>. <title>Slovnik řeči česke</title>. In: <title>Neuverbesserte Böhmische Grammatik, mit all erforderlichen tüchtigen Grundsätzen…</title><pubPlace>Vídeň (Wien)</pubPlace>, <publisher>Johann Thomas von Trattner</publisher><date>1783</date>, <biblScope unit="pp" from="283" to="470">283–470</biblScope>. <edition>Vydání páté. </edition>
                        <idno type="Knihopis">K14097</idno>
                        </bibl>
                    </p>
                </sourceDesc>
                <sourceDesc>
                    <msDesc xml:lang="cs">
                        <msIdentifier>
                            <country key="xr">Česko</country>
                            <settlement>Brno</settlement>
                            <repository>Moravská zemská knihovna</repository>
                            <idno>ST1-0006.461.A</idno>
                        </msIdentifier>
                        <msContents>
                            <msItem>
                                <title>Slovnik řeči česke</title>
                                <author><forename>Jan Václav</forename><surname>Pohl</surname></author>
                            </msItem>
                        </msContents>
                        <history>
                            <origin>
                                <origDate notBefore="1783" notAfter="1783">1783</origDate>
                            </origin>
                        </history>
                        <additional>
                            <adminInfo>
                                <recordHist>
                                    <p>
                                        <persName>Boris Lehečka</persName>, <date>21. 12. 2015</date>
                                        <note>základní převod na XML</note>
                                    </p>
                                </recordHist>
                            </adminInfo>
                        </additional>
                    </msDesc>
                </sourceDesc>
            </fileDesc>
            <xsl:call-template name="InsertEndocingDesc"/>
            <encodingDesc>
                <projectDesc>
                    <p>Viz Knihopis č. 14097 (<ref target="http://db.knihopis.org/l.dll?cll~14354">http://db.knihopis.org/l.dll?cll~14354</ref>), Jungmannova <hi rend="italic">Historie literatury české</hi> V. 28.</p>
                    <p>Elektronická podoba <hi rend="italic">Slovníku řeči česke</hi> Jana Václava Pohla (1720–1790) byla pořízena z pátého vydání jeho mluvnice, která vyšla pod názvem <hi rend="italic">Neuverbesserte Böhmische Grammatik, mit all erforderlichen tüchtigen Grundsätzen…</hi>. Do Vokabuláře webového mohla být zařazena díky vstřícnosti prof. Tilmana Bergera z univerzity v Tubinkách, jenž se svými spolupracovníky tuto elektronickou verzi Pohlova slovníku pořídil.</p>
                    <p>Vznik elektronické verze byl podpořen projektem Ministerstva kultury ČR č. DF12P01OVV028 <hi rend="italic">Informační technologie ve službách jazykového kulturního dědictví (IT JAKUB)</hi>.</p>
                    <p>Na tiskové chyby upozorňujeme v číslovaných poznámkách pod odstavcem.</p>
                    <p><hi rend="bold">Poznámky k transkripci češtiny</hi></p>
                    <p>Transliterovaný text slovníku Jana Václava Pohla byl transkribován do novočeského pravopisu pomocí automatického softwarového nástroje <ref target="http://vokabular.ujc.cas.cz/moduly/nastroje/transcriptorium/">Transcriptorium</ref>. Následovala kontrola této transkripce, kterou provedla Mgr. Pavlína Kuderová.</p>
                    <p>Zatímco první vydání slovníku z roku 1756 obsahuje slovní zásobu, kterou lze až na výjimky označit jako běžnou slovní zásobu češtiny 18. století, v pátém vydání slovníku z roku 1783 se tato slovní zásoba výrazně proměnila – kvantitativně i kvalitativně. Slovník obsahuje téměř třikrát více výrazů, z nichž mnohé nejsou zachyceny v jiných slovnících či textech. Často se jedná o novotvary, které nejsou vytvořeny ve shodě s českým slovotvorným systémem, či kalky; tyto výrazy se do českého jazyka více nerozšířily.</p>
                    <p>Často podivně vytvořená a nesrozumitelná slova nám v transkripci neumožňují spolehlivě rekonstruovat jejich lexikální formu (především s ohledem na vokalickou kvantitu), proto jejich vokalickou kvantitu dále neupravujeme (nesjednocujeme) – např. <hi rend="italic">bouřká</hi>, <hi rend="italic">týčká</hi>, <hi rend="italic">kosá</hi> atd. Zachováváme i další těžko interpretovatelné jazykové jevy, které mohou odrážet Pohlovy jazykové, zvláště ortografické ambice (např. předponu/násloví v podobě „zs“ – <hi rend="italic">zschody</hi>, <hi rend="italic">zsvlečky</hi>, <hi rend="italic">zstíti</hi> atd.). Odstranění těchto nezvyklých jevů by znemožnilo případný výzkum pravopisných návrhů, které se J. V. Pohl snažil do svých textů prosadit.</p>
                    <p>V transliterované části slovníku uvádíme výrazy doplněné editorem (ať již byly v původním slovníku naznačeny, či nikoli) v hranatých závorkách (např. Západo-poledni [Wjtr]). V transkripci tyto doplňky nijak nesignalizujeme.</p>
                    <p>Psaní velkých písmen v transkripci upravujeme podle současných pravidel pravopisu. Velké písmeno rovněž uvádíme na začátku řádku s novým heslem.</p>
                    <p>Za připravení internetové verze slovníku děkujeme RNDr. Pavlu Květoňovi, Ph.D., z Ústavu pro jazyk český AV ČR, v. v. i. </p>
                </projectDesc>
<!--                <editorialDecl>
                    <correction>
                        <p>Na několika místech je v závorce drobným kurzivním písmem upozorněno na tiskové chyby.</p>
                    </correction>
                </editorialDecl>
-->
            </encodingDesc>
            <profileDesc>
                <textClass>
                    <catRef target="#taxonomy-dictionary-contemporary #output-dictionary #taxonomy-historical_text-medieval_czech #output-editions"/>
                </textClass>
                <langUsage>
                    <language ident="cs" usage="80">čeština</language>
                    <language ident="de" usage="20">němčina</language>
                    <language ident="la" usage="10">latina</language>
                </langUsage>
            </profileDesc>
        </teiHeader>
    </xsl:template>
</xsl:stylesheet>