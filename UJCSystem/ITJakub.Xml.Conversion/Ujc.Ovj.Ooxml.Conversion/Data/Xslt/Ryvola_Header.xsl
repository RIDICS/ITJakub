<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xs xd tei"
    version="2.0">

  <xsl:include href="TEI_Common.xsl"/>
  
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> May 10, 2016</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>

  <xsl:output method="xml" encoding="UTF-8" indent="yes" omit-xml-declaration="no"/>
  <xsl:strip-space elements="*"/>
  <xsl:preserve-space elements="text"/>
  <xsl:variable name="vychozi-jazyk" select="'cs'"/>



  <xsl:template match="/">
    <xsl:comment> Ryvola_Header </xsl:comment>
    <TEI xmlns="http://www.tei-c.org/ns/1.0">
      <xsl:call-template name="insert-header" />
      <xsl:element name="text" namespace="http://www.tei-c.org/ns/1.0">
        <xsl:copy-of select="tei:body"/>
      </xsl:element>
    </TEI>
  </xsl:template>

  <xsl:template name="insert-header">
    <teiHeader xmlns="http://www.tei-c.org/ns/1.0" xml:id="Ryvola" n="Ryvola">
      <fileDesc n="{{465692DD-BD0A-43C5-9925-9CB07A15E958}}">
        <titleStmt>
          <title>Slovář český, to jest slova některá česká jak od Latinářův, tak i od Němcův vypůjčená, zase napravená a v vlastní českou řeč obrácená k užívání milovníkův české řeči</title>
          <author>
            <forename>Jan František Josef</forename>
            <surname>Ryvola</surname>
          </author>
        </titleStmt>
        <editionStmt>

          <edition>digitální slovník</edition>
          <respStmt>
            <resp>editor</resp>
            <name>Černá, Alena M.</name>
          </respStmt>
          <respStmt>
            <resp>editor</resp>
            <name>Oliva, Karel</name>
          </respStmt>
          <respStmt>
            <resp>editor</resp>
            <name>Lehečka, Boris</name>
          </respStmt>
          <respStmt>
            <resp>editor</resp>
            <name>Honková, Tereza</name>
          </respStmt>
          <respStmt>
            <resp>editor</resp>
            <name>Ulmanová, Kateřina</name>
          </respStmt>
          <respStmt>
            <resp>kódování TEI</resp>
            <name>Lehečka, Boris</name>
          </respStmt>
        </editionStmt>
        <publicationStmt>
          <publisher>
            oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email>
          </publisher>
          <pubPlace>Praha</pubPlace>
          <date>2007</date>
          <availability status="restricted">
            <p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
          </availability>
        </publicationStmt>
        <sourceDesc n="bibl">
          <p>
            <bibl type="source">
              <author>
                <forename>František Ladislav</forename>
                <surname>Čelakovský</surname>
              </author>. <title>Slovář český, to jest slova některá česká jak od Latinářův, tak i od Němcův vypůjčená, zase napravená a v vlastní českou řeč obrácená k užívání milovníkův české řeči</title>. <pubPlace>Praha</pubPlace> <date>xxx</date>.
            </bibl>
          </p>
        </sourceDesc>
        <sourceDesc>
          <msDesc xml:lang="cs">
            <msIdentifier>
              <country key="xr">Česko</country>
              <settlement>Praha</settlement>
              <repository>Ústav pro jazyk český AV ČR, v. v. i.</repository>
              <idno>XXXX</idno>
            </msIdentifier>
            <msContents>
              <msItem>
                <title>Dodavky ke slovníku Josefa Jungmanna</title>
                <author>
                  <forename>František Ladislav</forename>
                  <surname>Ryvola</surname>
                </author>
              </msItem>
            </msContents>
            <history>
              <origin>
                <origDate notBefore="1851" notAfter="1851">1851</origDate>
              </origin>
            </history>
            <additional>
              <adminInfo>
                <recordHist>
                  <p>
                    <persName>Boris Lehečka</persName>, <date>10. 5. 2015</date>
                    <note>základní převod na XML TEI P5</note>
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
          <p>	Elektronický text Čelakovského Dodavků byl dokončen v roce 2007; podíleli se na něm Alena M. Černá, Boris Lehečka, Karel Oliva a studentky FF UK v Praze Tereza Honková a Kateřina Ulmanová.</p>
        </projectDesc>
      </encodingDesc>
      <profileDesc>
        <textClass>
          <catRef target="#taxonomy-dictionary-contemporary #output-dictionary"/>
        </textClass>
        <langUsage>
          <language ident="cs" usage="80">čeština</language>
          <language ident="de" usage="15">němčina</language>
          <language ident="la" usage="5">latina</language>
        </langUsage>
      </profileDesc>
    </teiHeader>
  </xsl:template>
</xsl:stylesheet>