<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xd tei" version="1.0">
    
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
        <xsl:comment> PohlSlov1756_Hlavicka </xsl:comment>
        <xsl:text xml:space="preserve">
</xsl:text>
        <TEI xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:call-template name="insert-header" />
            <tei:text>
                <xsl:copy-of select="tei:body"/>
            </tei:text>
        </TEI>
    </xsl:template>
    
    <xsl:template name="insert-header">
        <teiHeader xmlns="http://www.tei-c.org/ns/1.0" xml:id="PohlSlov1756" n="PohlSlov1756">
            <fileDesc n="{{1C7772C7-BDF5-4378-8992-42232980AF44}}">
                <titleStmt>
                    <title type="translit">Česko-německý slovnář oder Böhmisch-Deutsches Wörter-Buch</title>
                    <author>
                        <forename>Jan Václav</forename>
                        <surname>Pohl</surname>
                    </author>
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
                        <bibl type="source"><author><forename>Jan Václav</forename> <surname>Pohl</surname></author>. <title>Česko-německý slovnář oder Böhmisch-Deutsches Wörter-Buch</title>. In: <title>Grammatica linguae Bohemicae oder Die böhmische Sprachkunst, bestehend in vier Theilen…</title><pubPlace>Vídeň; Praha; Terst (Wien, Prag und Triest)</pubPlace>, <publisher>Johann Thomas von Trattner</publisher><date>1756</date>, <biblScope unit="pp" from="205" to="258">205–258</biblScope>. <edition>Vydání první. </edition>
                        <idno type="Knihopis">K14094</idno>
                        </bibl>
                    </p>
                </sourceDesc>
                <sourceDesc>
                    <msDesc xml:lang="cs">
                        <msIdentifier>
                            <repository>ze soukromé sbírky</repository>
                        </msIdentifier>
                        <msContents>
                            <msItem>
                                <title>Česko-německý slovnář oder Böhmisch-Deutsches Wörter-Buch</title>
                                <author><forename>Jan Václav</forename><surname>Pohl</surname></author>
                            </msItem>
                        </msContents>
                        <history>
                            <origin>
                                <origDate notBefore="1756" notAfter="1756">1756</origDate>
                            </origin>
                        </history>
                        <additional>
                            <adminInfo>
                                <recordHist>
                                    <p>
                                        <persName>Boris Lehečka</persName>, <date>15. 7. 2015</date>
                                        <note>základní převod na XML</note>
                                    </p>
                                </recordHist>
                            </adminInfo>
                        </additional>
                    </msDesc>
                </sourceDesc>
            </fileDesc>
            <encodingDesc>
                <projectDesc>
                    <p>Viz Knihopis č. 14094 (<ref target="http://db.knihopis.org/l.dll?cll~14351">http://db.knihopis.org/l.dll?cll~14351</ref>), Jungmannova <hi rend="italic">Historie literatury české</hi> V. 28.</p>
                    <p>Elektronická podoba <hi rend="italic">Česko-německého slovnáře</hi> Jana Václava Pohla (1720–1790) byla pořízena z prvního vydání Pohlovy mluvnice <hi rend="italic">Grammatica linguae Bohemicae</hi>. Do Vokabuláře webového mohla být zařazena díky vstřícnosti prof. Tilmana Bergera z univerzity v Tubinkách, jenž se svými spolupracovníky tuto elektronickou verzi Pohlova slovníku pořídil.</p>
                    <p>Vznik elektronické verze byl podpořen projektem Ministerstva kultury ČR č. DF12P01OVV028 <hi rend="italic">Informační technologie ve službách jazykového kulturního dědictví (IT JAKUB)</hi>.</p>
                    <p>Na tiskové chyby upozorňujeme v číslovaných poznámkách pod odstavcem.</p>
                    <p><hi rend="bold">Poznámky k transkripci češtiny</hi></p>
                    <p>Transliterovaný text slovníku Jana Václava Pohla byl transkribován do novočeského pravopisu pomocí automatického softwarového nástroje <hi rend="italic">Transcriptorium</hi>. Následovala kontrola této transkripce, kterou provedla Mgr. Pavlína Kuderová.</p>
                    <p>Při transkripci bylo problematické rozhodnout, zda odchylky od standardního pravopisného (tiskařského) úzu jsou nedostatky tisku, které by bylo náležité v elektronické verzi slovníku opravit, či zda se jedná o aplikaci pravopisných návrhů Jana Václava Pohla. Tyto pravopisné teorie mohly najít odraz v tiscích Pohlových děl, i když tento odraz zároveň mohl být zkreslen normalizačními zásahy tiskařů, korektorů a jiných osob, které se podílely na výsledné podobě knihy.</p>
                    <p>Odchylky od pravopisného úzu shledáváme především v oblasti vokalické kvantity. Opravujeme pouze nepochybně chybné zápisy; v ostatních případech ponecháváme v transkripci délku či krátkost vokálů, které jsou uvedené v tisku, abychom případným badatelům neznemožnili další výzkum Pohlova pravopisu, případně i tiskařského úzu druhé poloviny 18. století. Zachováváme na příklad zaznamenanou krátkost v koncovce nominativu složených adjektiv (zejména ženského rodu) ve skladební pozici shodného přívlastku (např. <hi rend="italic">Května neděle</hi>, <hi rend="italic">Bíla sobota</hi>, <hi rend="italic">Červena řepa</hi>, <hi rend="italic">Punčocha hedbávna</hi>; <hi rend="italic">Mrtve tělo</hi>; <hi rend="italic">Teleci maso</hi>; <hi rend="italic">spůsobnosti lidske</hi>). V některých případech se nestandardní vokalická kvantita českých výrazů z Pohlova slovníku shoduje s nestandardní kvantitou doloženou i v jiných zdrojích (např. <hi rend="italic">Sýn</hi>, <hi rend="italic">Kúře</hi>; důkazem, že taková kvantita je autorský záměr, dokládá i opakování stejné lexikální formy v tomto slovníku), dále <hi rend="italic">Pívo</hi>, <hi rend="italic">Polivka</hi>, <hi rend="italic">Zajic</hi>, <hi rend="italic">Chleb</hi>, <hi rend="italic">Žíto </hi>aj. Kolísání ponecháváme také v afixech, u nichž je rovněž doloženo z jiných zdrojů, neboť může odrážet vokalickou kvantitu mluvené řeči či kvantitu nářeční (např. <hi rend="italic">Hodinař</hi>, <hi rend="italic">Vystavnice</hi>, <hi rend="italic">Zastěra</hi>, <hi rend="italic">Koštíště</hi>, <hi rend="italic">Ohníště</hi> aj.).</p>
                    <p>V transkripci zachováváme i další specifické jazykové rysy, např. podobu <hi rend="italic">Očím </hi><hi rend="italic">Stief-Vatter</hi>, která může být výsledkem zjednodušení souhláskové skupiny, avšak nelze vyloučit ani chybu tisku. Zachováváme i psaní podob vzniklých asimilací (např. <hi rend="italic">Potkova</hi>, <hi rend="italic">Těškomyslnost</hi>).</p>
                    <p>V transkripci opravujeme interpunkci, která byla v tisku v některých případech užita nadbytečně a sloužila pouze k automatickému oddělení dvou sousedních slov, např. Swatoduſſnj, Swátky upravujeme na <hi rend="italic">Svatodušní svátky</hi>, Kadeřave, kudrnate, vlasy na <hi rend="italic">Kadeřave, kudrnate vlasy</hi>). Zároveň v transkribované části doplňujeme interpunkční čárku tam, kde v tisku omylem vytištěna nebyla (např. výraz minułý předeſſlý Rok transkribujeme <hi rend="italic">Minulý, předešlý rok</hi>).</p>
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
                <langUsage>
                    <language ident="cs" usage="80">čeština</language>
                    <language ident="de" usage="20">němčina</language>
                    <language ident="la" usage="10">latina</language>
                </langUsage>
            </profileDesc>
        </teiHeader>
    </xsl:template>
</xsl:stylesheet>