<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0"
	xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema"
	exclude-result-prefixes="xd xsd"
	version="2.0">
  <xd:doc scope="stylesheet">
    <xd:desc>
      <xd:p>
        <xd:b>Created on:</xd:b> Aug 20, 2015
      </xd:p>
      <xd:p>
        <xd:b>Author:</xd:b> lehecka
      </xd:p>
      <xd:p></xd:p>
    </xd:desc>
  </xd:doc>

  <xsl:param name="versionId" />

  <xsl:output method="xml" indent="yes" encoding="UTF-8" />

  <xsl:template match="/">
    <itj:document doctype="Grammar" versionId="{$versionId}" xml:lang="cs" n="{/tei:TEI/tei:teiHeader/tei:fileDesc/@n}"
			xmlns="http://www.tei-c.org/ns/1.0"
			xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0"
			xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
			xmlns:xml="http://www.w3.org/XML/1998/namespace">

      <xsl:apply-templates select="tei:TEI/tei:teiHeader" />

      <xsl:call-template name="terms" />
      <xsl:call-template name="pages" />
      <xsl:call-template name="accessories" />

    </itj:document>
  </xsl:template>

  <xsl:template match="tei:sourceDesc[not(@n)]">
    <xsl:copy-of select="." />
    <sourceDesc xmlns="http://www.tei-c.org/ns/1.0">
      <listBibl>
        <bibl type="acronym" subtype="source">
          <xsl:value-of select="tei:TEI/tei:teiHeader/@n"/>
        </bibl>
      </listBibl>
    </sourceDesc>
  </xsl:template>


  <xsl:template match="tei:fileDesc">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="tei:profileDesc">
    <xsl:copy>
      <xsl:copy-of select="@*" />
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>

  <xsl:template name="terms">
    <itj:terms>
      <xsl:for-each-group select="//tei:term" group-by="@n">
        <xsl:sort select="@n" />
        <xsl:copy-of select="."/>
        <!--<xsl:apply-templates select="." mode="terms" />-->
      </xsl:for-each-group>
    </itj:terms>
  </xsl:template>

  <xsl:template name="pages">
    <itj:pages>
      <xsl:apply-templates select="//tei:facsimile/tei:surface" />
    </itj:pages>
  </xsl:template>

  <xsl:template name="accessories">
    <itj:accessories>
      <itj:cover facs="{/tei:TEI/tei:teiHeader/@xml:id}.jpg" />
    </itj:accessories>
  </xsl:template>

  <xsl:template match="tei:surface">
    <itj:page n="{@n}" facs="{tei:graphic/@url}">
      <xsl:apply-templates select="tei:desc/tei:term" mode="pages" />
    </itj:page>
  </xsl:template>


  <xsl:template match="tei:term" mode="pages">
    <itj:termRef n="{@id}" />
  </xsl:template>

  <xsl:template match="tei:term" mode="terms">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>


  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>