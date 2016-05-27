<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
    xmlns="http://www.tei-c.org/ns/1.0" 
    xmlns:xml="http://www.w3.org/XML/1998/namespace" 
    exclude-result-prefixes="xd" 
    version="1.0">
    
    <xsl:include href="TEI_Common.xsl"/>
    
    <xsl:param name="guid" />
    
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Jun 11, 2012</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p>Převede základní XML z Wordu na hlavičku TEI</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:output encoding="UTF-8" method="xml" indent="yes"/>
    <xsl:strip-space elements="*"/>

    <xsl:template match="body">
        <xsl:variable name="zkratka">
            <xsl:value-of select="table[2]/row[4]/cell[2]/Normalni[1]/zkratka_mluvnice/text()"/>
        </xsl:variable>

        <xsl:variable name="identifikator">
            <xsl:call-template name="RemoveDiacritics">
                <xsl:with-param name="text" select="$zkratka"/>
            </xsl:call-template>
        </xsl:variable>

        <TEI xmlns="http://www.tei-c.org/ns/1.0">
            <teiHeader>
                <xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
                    <xsl:value-of select="$identifikator"/>
                </xsl:attribute>
                <xsl:attribute name="n">
                    <xsl:value-of select="$zkratka"/>
                </xsl:attribute>
                <fileDesc n="{$guid}">
                    <titleStmt>
                        <title type="translit">
                            <xsl:apply-templates select="table[2]/row[2]/cell[2]/Normalni[1]"/>
                        </title>
                        <title>
                            <xsl:apply-templates select="table[2]/row[1]/cell[2]/Normalni[1]"/>
                            <xsl:if test="table[2]/row[5]/cell[2]/Normalni[1]//text()">
                                <note>
                                    <xsl:apply-templates select="table[2]/row[5]/cell[2]"/>
                                </note>
                            </xsl:if>
                        </title>
                        <!-- uzuálí titul -->
                        <xsl:apply-templates select="table[2]/row[3]/cell[2]"/>
                        <xsl:apply-templates select="table[1]/row[@n > 1]" mode="autor"/>
                        <!--<author><forename>Ondřej</forename><surname>Klatovský z Dalmanhorstu</surname></author>-->
                    </titleStmt>
                    <editionStmt>
                        <edition>digitální mluvnice</edition>
                    </editionStmt>
                    <publicationStmt>
                        <publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email></publisher>
                        <pubPlace>Praha</pubPlace>
                        <date>2012</date>
                        <availability status="restricted">
                            <p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
                        </availability>
                    </publicationStmt>
                    <sourceDesc n="format">
                        <xsl:apply-templates select="table[@n='5']"/>
                    </sourceDesc>
                    <sourceDesc n="bibl">
                        <p>
                            <bibl type="source">
                                <xsl:apply-templates select="table[1]/row[@n > 1]" mode="autor-bibl"/>
                                <xsl:if test="string-length(table[1]/row[2]/cell[1]//text()) > 0 or (table[1]/row[2]/cell[1]//text() != 'anonym')">
                                    <!--<xsl:text>: </xsl:text>--><xsl:text>. </xsl:text> <!-- tečka mezi autory a titulem na přání OK -->
                                </xsl:if>
                                <title>
                                    <xsl:apply-templates select="table[2]/row[1]/cell[2]/Normalni[1]"/>
                                </title>
                                <xsl:apply-templates select="table[2]/row[3]/cell[2]"/>
                                <xsl:apply-templates select="table[3]" mode="tisk-bibl"/>
                                <xsl:text> </xsl:text>
                                <xsl:call-template name="knihopis"/>
                            </bibl>
                            <!--                            <bibl type="source"><author><forename>Ondřej</forename><surname>Klatovský z Dalmanhorstu</surname></author>: <title>Knížka v českém a německém jazyku složená, kterak by Čech německy a Němec česky čísti, psáti i mluviti učiti se měl</title>, <pubPlace>Olomouc</pubPlace>, <publisher>Jan Günther</publisher> <date>1564</date>.<idno type="Knihopis">K03940</idno></bibl>
-->
                        </p>
                    </sourceDesc>
                    <sourceDesc n="characteristic">
                        <xsl:apply-templates select="Charakteristika"/>
                    </sourceDesc>
                    <sourceDesc n="bibl-primary">
                        <xsl:apply-templates select="table[@n='7']"/>
                    </sourceDesc>
                    <sourceDesc n="bibl-secondary">
                        <xsl:apply-templates select="table[@n='8']"/>
                    </sourceDesc>
                    <sourceDesc>
                        <msDesc xml:lang="cs">
                            <msIdentifier>
                                <xsl:apply-templates select="table[@n = '4']" mode="identifier"/>
                                <!-- <country key="xr">Česko</country>
                                <settlement>Praha</settlement>
                                <repository>Knihovna Národního muzea v Praze</repository>
                                <idno>26 E 8</idno>-->
                            </msIdentifier>
                            <msContents>
                                <msItem>
                                    <title>
                                        <xsl:apply-templates select="table[2]/row[1]/cell[2]/Normalni[1]"/>
                                    </title>
                                    <xsl:apply-templates select="table[1]/row[@n > 1]" mode="autor"/>
                                    <!--                                    <title>Knížka v českém a německém jazyku složená, kterak by Čech německy a Němec česky čísti, psáti i mluviti učiti se měl</title>
                                    <author>
                                        <forename>Ondřej</forename>
                                        <surname>Klatovský z Dalmanhorstu</surname>
                                    </author>-->

                                </msItem>
                            </msContents>
                            <physDesc>
                                <objectDesc>
                                    <supportDesc>
                                        <xsl:apply-templates select="table[4]" mode="rozsah"/>
                                    </supportDesc>
                                </objectDesc>
                            </physDesc>
                            <history>
                                <origin>
                                    <!--<origDate notBefore="1564" notAfter="1564">1564</origDate>-->
                                    <xsl:call-template name="datace"/>
                                </origin>
                            </history>
                            <additional>
                                <adminInfo>
                                    <recordHist>
                                        <xsl:apply-templates select="table[11]"/>
                                    </recordHist>
                                </adminInfo>
                            </additional>
                        </msDesc>
                    </sourceDesc>
                </fileDesc>
                <xsl:call-template name="InsertEndocingDesc"/>
                <profileDesc>
                    <langUsage>
                        <xsl:apply-templates select="table[@n='9']"/>
                    </langUsage>
                    <textClass>
                        <catRef target="#taxonomy-digitized-grammar #output-digitized-grammar"/>
                    </textClass>
                </profileDesc>
            </teiHeader>
            <!--            <text>
                <front>
                    <div type="characteristic">
                        <xsl:apply-templates select="Charakteristika" />
                    </div>
                    <div type="bibl" subtype="primary">
                        <head><hi rend="capital">Primární literatura</hi>  <xsl:apply-templates select="Upresneni[1]" /></head>
                        <xsl:apply-templates select="table[@n='7']" />
                    </div>
                    <div type="bibl" subtype="secondary">
                        <head><hi rend="capital">Sekundární literatura</hi> <xsl:apply-templates select="Upresneni[2]" /></head>
                        <xsl:apply-templates select="table[@n='8']" />
                    </div>
                </front>
            </text>-->
        </TEI>
    </xsl:template>
    <xsl:template name="RemoveDiacritics">
        <xsl:param name="text"/>
        <xsl:value-of select="translate($text, 'áäčďéěíľłóöřšťúůüýžÁÄČĎÉĚÍĽŁÓÖŘŠŤÚŮÜÝŽ', 'aacdeeilloorstuuuyzAACDEEILLOORSTUUUYZ')"/>
    </xsl:template>


    <xsl:template match="Charakteristika">
        <p>
            <xsl:apply-templates/>
        </p>
    </xsl:template>

    <xsl:template match="predel">
        <anchor type="predel" n="{normalize-space(text())}"/>
        <xsl:if test="string-length(text()) > string-length(normalize-space(text()))">
            <xsl:text> </xsl:text>
        </xsl:if>
    </xsl:template>

    <xsl:template match="text">
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="bibliogaficky_odkaz">
        <ref target="{translate(text(), ' ', '')}" type="bibl">
            <xsl:apply-templates/>
        </ref>
    </xsl:template>

    <xsl:template match="zkratka_mluvnice">
        <xsl:variable name="zkratka">
            <xsl:call-template name="RemoveDiacritics">
                <xsl:with-param name="text">
                    <xsl:value-of select="text()"/>
                </xsl:with-param>
            </xsl:call-template>
        </xsl:variable>
        <ref target="{$zkratka}" type="mluvnice">
            <xsl:apply-templates/>
        </ref>
    </xsl:template>

    <xsl:template match="odkaz">
        <ref target="{ normalize-space(text())}" type="http">
            <xsl:apply-templates/>
        </ref>
    </xsl:template>

    <xsl:template match="pagina">
        <xsl:variable name="cil">
            <xsl:value-of select="translate(text(), '[]', '')"/>
        </xsl:variable>
        <ref target="{normalize-space($cil)}" type="pagina">
            <xsl:apply-templates/>
        </ref>
    </xsl:template>

    <xsl:template match="kurziva">
        <hi rend="italic">
            <xsl:apply-templates/>
        </hi>
    </xsl:template>

    <xsl:template match="tucne">
        <hi rend="bold">
            <xsl:apply-templates/>
        </hi>
    </xsl:template>

    <xsl:template match="horni_index">
        <hi rend="superscript">
            <xsl:apply-templates/>
        </hi>
    </xsl:template>

    <xsl:template match="prijmeni">
        <hi rend="capital">
            <xsl:apply-templates/>
        </hi>
    </xsl:template>

    <xd:doc>
        <xd:desc>Uzuální titul</xd:desc>
    </xd:doc>

    <xsl:template match="table[2]/row[3]/cell[2]">
        <xsl:if test="//text()">
            <title type="usual">
                <xsl:apply-templates select="Normalni"/>
            </title>
        </xsl:if>
    </xsl:template>

    <xd:doc>
        <xd:desc>Primární a sekundární literatura</xd:desc>
    </xd:doc>
    <xsl:template match="table[@n='7']">
        <listBibl>
            <head>
                <!--<hi rend="capital">Primární literatura </hi>-->
                <hi rend="beginning">Primární literatura </hi>
                <xsl:if test="string-length(preceding-sibling::node()[1]) &gt; 0"><xsl:text>(</xsl:text></xsl:if>
                <xsl:apply-templates select="preceding-sibling::node()[1]"/>
                <xsl:if test="string-length(preceding-sibling::node()[1]) &gt; 0"><xsl:text>)</xsl:text></xsl:if>
            </head>
            <xsl:for-each select="row[@n > 1]">
                <bibl>
                    <xsl:call-template name="BibliografickyOdkazAtributy"/>
                    <xsl:apply-templates select="cell[2]"/>
                </bibl>
            </xsl:for-each>
        </listBibl>
    </xsl:template>

    <xsl:template match="table[@n='8']">
        <listBibl>
            <head>
                <hi rend="beginning">Sekundární literatura </hi>
                <xsl:if test="string-length(preceding-sibling::node()[1]) &gt; 0"><xsl:text>(</xsl:text></xsl:if>
                <xsl:apply-templates select="preceding-sibling::node()[1]"/>
                <xsl:if test="string-length(preceding-sibling::node()[1]) &gt; 0"><xsl:text>)</xsl:text></xsl:if>
            </head>
            <xsl:for-each select="row[@n > 1]">
                <bibl>
                    <xsl:call-template name="BibliografickyOdkazAtributy"/>
                    <xsl:apply-templates select="cell[2]"/>
                </bibl>
            </xsl:for-each>
        </listBibl>
    </xsl:template>
    
    <xsl:template name="BibliografickyOdkazAtributy">
        <xsl:if test="cell[1]/Normalni/bibliogaficky_odkaz">
            <xsl:attribute name="n">
                <xsl:value-of select="cell[1]/Normalni/bibliogaficky_odkaz"/>
            </xsl:attribute>
            <xsl:attribute name="xml:id">
                <xsl:value-of select="translate(cell[1]/Normalni/bibliogaficky_odkaz, ' ', '')"/>
            </xsl:attribute>
        </xsl:if>
    </xsl:template>

    <xd:doc>
        <xd:desc>Formát tisku</xd:desc>
    </xd:doc>
    <xsl:template match="table[@n='5']">
        <p>
            <xsl:apply-templates select="row[1]/cell[2]"/>
            <xsl:if test="row[2]/cell[2]//text()">
                <xsl:text> </xsl:text>
                <note>
                    <xsl:apply-templates select="row[2]/cell[2]"/>
                </note>
            </xsl:if>
        </p>
    </xsl:template>

    <xsl:template match="table[@n='9']">
        <!--  <language ident="cs" usage="50">čeština</language>
              <language ident="de" usage="50">němčina</language>
        -->

        <xsl:variable name="pocet">
            <xsl:value-of select="format-number(100 div count(row), '###')"/>
        </xsl:variable>

        <xsl:for-each select="row[position() > 1]">
            <xsl:variable name="zkratka">
                <xsl:choose>
                    <xsl:when test=".//text() = 'čeština'">
                        <xsl:text>cs</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'němčina'">
                        <xsl:text>de</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'řečtina'">
                        <xsl:text>gr</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'latina'">
                        <xsl:text>la</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'polština'">
                        <xsl:text>pl</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'ruština'">
                        <xsl:text>ru</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'církevní slovanština'">
                        <xsl:text>cu</xsl:text>
                    </xsl:when>
                    <xsl:when test=".//text() = 'jihoslovanské jazyky'">
                        <xsl:value-of select="''"/>
                    </xsl:when>
                </xsl:choose>
            </xsl:variable>
            <xsl:if test="not($zkratka = '')">
                <language ident="{$zkratka}" usage="{$pocet}">
                    <xsl:apply-templates select="./cell[@n='1']"/>
                </language>
            </xsl:if>
        </xsl:for-each>
    </xsl:template>

    <xsl:template match="table[@n='1']/row[@n > 1]" mode="autor-bibl">
        <xsl:if test="string-length(cell[@n = 1]//text()) > 0 or (cell[@n = 1]//text() != 'anonym')">
            <author>
                <forename>
                    <xsl:apply-templates select="cell[@n = 1]"/>
                </forename>
                <xsl:if test="cell[@n = 2]//text()">
                    <forename type="variant">
                        <xsl:apply-templates select="cell[@n = 2]"/>
                    </forename>
                </xsl:if>
                <xsl:text>, </xsl:text>
                <surname>
                    <xsl:apply-templates select="cell[@n = 3]"/>
                </surname>
                <xsl:if test="cell[@n = 5]//text()">
                    <surname type="variant">
                        <xsl:apply-templates select="cell[@n = 5]"/>
                    </surname>
                </xsl:if>
            </author>
            <xsl:if test="not(position() = last())">
                <xsl:text> – </xsl:text>
            </xsl:if>
        </xsl:if>
    </xsl:template>


    <xsl:template match="table[@n='1']/row[@n > 1]" mode="autor">
        <xsl:if test="string-length(cell[@n = 1]//text()) > 0 or (cell[@n = 1]//text() != 'anonym')">
            <author>
                <forename>
                    <xsl:apply-templates select="cell[@n = 1]"/>
                </forename>
                <xsl:if test="cell[@n = 2]//text()">
                    <forename type="variant">
                        <xsl:apply-templates select="cell[@n = 2]"/>
                    </forename>
                </xsl:if>
                <surname>
                    <xsl:apply-templates select="cell[@n = 3]"/>
                </surname>
                <xsl:if test="cell[@n = 5]//text()">
                    <surname type="variant">
                        <xsl:apply-templates select="cell[@n = 5]"/>
                    </surname>
                </xsl:if>
            </author>
        </xsl:if>
    </xsl:template>

    <xsl:template match="fraktura | svabach | antikva">
        <hi rend="{name()}">
            <xsl:apply-templates/>
        </hi>
    </xsl:template>

    <xsl:template match="knihopis">
        <ref target="{text()}" type="knihopis">
            <xsl:apply-templates/>
        </ref>
    </xsl:template>

    <xsl:template match="table[@n='3']" mode="tisk-bibl">
        <xsl:if test="string-length(row[@n='1']/cell[@n='2']) &gt; 0">
            <pubPlace><xsl:apply-templates select="row[@n='1']/cell[@n='2']"/></pubPlace>    
        </xsl:if>

        <xsl:if test="string-length(row[@n='2']/cell[@n='2']) &gt; 0">
            <xsl:text>, </xsl:text>
            <publisher><xsl:apply-templates select="row[@n='2']/cell[@n='2']"/></publisher>
        </xsl:if>
        <xsl:text> </xsl:text>
        <date><xsl:apply-templates select="row[@n='3']/cell[@n='2']"/></date>
        <xsl:text>. </xsl:text>
        <xsl:if test="string-length(row[@n='4']/cell[@n='2']) &gt; 0">
            <edition><xsl:text>Vydání </xsl:text><xsl:apply-templates select="row[@n='4']/cell[@n='2']"/><xsl:text>. </xsl:text></edition>
        </xsl:if>
        <xsl:if test="row[@n='5']/cell[@n='2']//text()">
            <note>
                <xsl:apply-templates select="row[@n='5']/cell[@n='2']"/>
            </note>
        </xsl:if>
    </xsl:template>

    <xsl:template name="knihopis">
        <xsl:if test="string-length(/body/table[@n ='6']/row[@n ='1']/cell[@n ='2']) &gt; 0">
            <idno type="Knihopis">
                <xsl:value-of select="/body/table[@n ='6']/row[@n ='1']/cell[@n ='2']"/>
            </idno>
        </xsl:if>
        <xsl:if test="string-length(/body/table[@n ='6']/row[@n ='2']/cell[@n ='2']) &gt; 0">
            <idno type="NKCR">
                <!-- Odstranění špičatých závorek, pokud jsou součástí textu identifikátoru -->
                <xsl:value-of select="translate(/body/table[@n ='6']/row[@n ='2']/cell[@n ='2'], '&lt;&gt;', '')"/>
            </idno>
        </xsl:if>
    </xsl:template>

    <xsl:template match="table[4]" mode="rozsah">
        <extent>
            <xsl:apply-templates select="row[5]/cell[2]"/>
            <xsl:if test="row[6]/cell[2]//text()">
                <note>
                    <xsl:apply-templates select="row[6]/cell[2]"/>
                </note>
            </xsl:if>
        </extent>
    </xsl:template>

    <xsl:template match="table[@n='4']" mode="identifier">
        <!--        <country key="xr">Česko</country>
        <settlement>Praha</settlement>
        <repository>Knihovna Národního muzea v Praze</repository>
        <idno>26 E 8</idno>
-->

        <xsl:if test="string-length(row[@n='1']/cell[@n='2']) &gt; 0">
            <country key="xr">
                <xsl:apply-templates select="row[@n='1']/cell[@n='2']"/>
            </country>
        </xsl:if>

        <xsl:if test="string-length(row[@n='2']/cell[@n='2']) &gt; 0">
            <settlement>
                <xsl:apply-templates select="row[@n='2']/cell[@n='2']"/>
            </settlement>
        </xsl:if>
        <xsl:if test="string-length(row[@n='3']/cell[@n='2']) &gt; 0">
            <repository>
                <xsl:apply-templates select="row[@n='3']/cell[@n='2']"/>
            </repository>
        </xsl:if>
        <xsl:if test="string-length(row[@n='4']/cell[@n='2']) &gt; 0">
            <idno>
                <xsl:apply-templates select="row[@n='4']/cell[@n='2']"/>
            </idno>
        </xsl:if>
    </xsl:template>

    <xsl:template name="datace">
        <xsl:variable name="rok">
            <xsl:apply-templates select="table[@n='3']/row[@n='3']/cell[@n='2']"/>
        </xsl:variable>
        <xsl:choose>
            <!-- Dočasné řešení; týká se HusAbec -->
            <xsl:when test="$rok = '15. století'">
                <origDate notBefore="1400" notAfter="1499">
                    <xsl:value-of select="$rok"/>
                </origDate>
            </xsl:when>
            <xsl:otherwise>
                <origDate notBefore="{$rok}" notAfter="{$rok}">
                    <xsl:value-of select="$rok"/>
                </origDate>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template match="table[11]">
        <xsl:for-each select="row[position() > 1]">
            <p><persName><xsl:apply-templates select="cell[1]"/></persName>, <date><xsl:apply-templates select="cell[2]"/></date>
                <xsl:if test="cell[3]//text()">
                    <note>
                        <xsl:apply-templates select="cell[3]"/>
                    </note>
                </xsl:if>
            </p>
        </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>
