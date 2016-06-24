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
  
  <xsl:param name="guid" />
    
    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
    <xsl:preserve-space elements="text"/>
    <xsl:variable name="vychozi-jazyk" select="'cs'"/>
    
    
    
    <xsl:template match="/">
        <xsl:text xml:space="preserve">
</xsl:text>
        <xsl:comment> DDBW_Head </xsl:comment>
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
        <teiHeader xmlns="http://www.tei-c.org/ns/1.0" xml:id="DDBW" n="DDBW">
            <fileDesc n="{$guid}">
              <titleStmt>
                <title>Deutsch-böhmisches Wörterbuch</title>
                <author>
                  <forename>Josef</forename>
                  <surname>Dobrovský</surname>
                </author>
              </titleStmt>
              <editionStmt>
                <edition>digitální slovník</edition>
              </editionStmt>
              <publicationStmt>
                <publisher>
                  oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email>
                </publisher>
                <pubPlace>Praha</pubPlace>
                <date>2011</date>
                <availability status='restricted'>
                  <p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
                </availability>
              </publicationStmt>
              <sourceDesc n='bibl'>
                <p>
                  <bibl type='source'>
                    <author>
                      <forename>Josef</forename>, <surname>Dobrovský</surname>
                    </author>. <title>Deutsch-böhmisches Wörterbuch</title> <pubPlace>Praha</pubPlace>, <publisher></publisher> <date></date>. <edition></edition>
                  </bibl>
                </p>
              </sourceDesc>
              <sourceDesc>
                <msDesc xml:lang='cs'>
                  <msIdentifier>
                    <country key='xr'>Česko</country>
                    <settlement>Praha</settlement>
                    <repository>Ústav pro jazyk český AV ČR, v. v. i.</repository>
                    <idno>0000</idno>
                  </msIdentifier>
                  <msContents>
                    <msItem>
                      <title>Deutsch-böhmisches Wörterbuch</title>
                      <author>
                        <forename>Josef</forename>
                        <surname>Dobrovský</surname>
                      </author>
                    </msItem>
                  </msContents>
                  <history>
                    <origin>
                      <origDate notBefore='1821' notAfter='1821'>1821</origDate>
                    </origin>
                  </history>
                  <additional>
                    <adminInfo>
                      <recordHist>
                        <p>
                          <persName>Boris Lehečka</persName>, <date>{0}</date><note>základní převod na XML</note>
                        </p>
                      </recordHist>
                    </adminInfo>
                  </additional>
                </msDesc>
              </sourceDesc>
            </fileDesc>
            <xsl:call-template name="InsertEndocingDesc"/>
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