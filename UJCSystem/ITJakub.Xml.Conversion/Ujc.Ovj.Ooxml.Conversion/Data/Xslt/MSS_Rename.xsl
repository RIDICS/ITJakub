<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:t="http://schema.brus.cz/2010/WDoc2Xml.xsd"
                version="1.0">
  <xsl:output indent="yes"/>
  <xsl:template match="/">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="body">
    <xsl:element name="dictionary">
      <xsl:attribute name="name">
        <xsl:text>MSS</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="Pismeno">
    <xsl:element name="milestone">
      <xsl:attribute name="type">
        <xsl:text>letter</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="Heslovy_odkaz">
    <xsl:element name="entry">
      <xsl:attribute name="type">
        <xsl:text>ref</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="Heslovy_odstavec">
    <xsl:element name="entry">
      <xsl:attribute name="type">
        <xsl:text>full</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="delimitator">
    <xsl:element name="delim">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="heslove_slovo">
    <xsl:element name="hw">
      <xsl:attribute name="rend">
        <xsl:text>bo</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="heslove_slovo_delimitator">
    <xsl:element name="delim">
      <xsl:attribute name="rend">
        <xsl:text>bo</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="heslove_slovo_zkracene_delimitator">
    <xsl:element name="delim">
      <xsl:attribute name="rend">
        <xsl:text>bo</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="form">
        <xsl:text>short</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="heslove_slovo_rozepsane">
    <xsl:element name="hw">
      <xsl:attribute name="rend">
        <xsl:text>bo</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="form">
        <xsl:text>restored</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="heslove_slovo_zkracene">
    <xsl:element name="hw">
      <xsl:attribute name="rend">
        <xsl:text>bo</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="form">
        <xsl:text>short</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="nonparej">
    <xsl:element name="text">
      <xsl:attribute name="rend">
        <xsl:text>nonp</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="nonparej_delimitator">
    <xsl:element name="delim">
      <xsl:attribute name="rend">
        <xsl:text>nonp</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="nonparej_charakteristika">
    <xsl:element name="gloss">
      <xsl:attribute name="rend">
        <xsl:text>nonp</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="slovni_druh">
    <xsl:element name="pos">
      <xsl:attribute name="rend">
        <xsl:text>nonp</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="nonparej_morfologie">
    <xsl:element name="gloss">
      <xsl:attribute name="rend">
        <xsl:text>nonp</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="type">
        <xsl:text>morph</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="odkazovane_heslo">
    <xsl:element name="xref">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="rozepsane_delimitator">
    <xsl:element name="text">
      <xsl:attribute name="form">
        <xsl:text>restored</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="rozepsane_delimitator_tucne">
    <xsl:element name="text">
      <xsl:attribute name="rend">
        <xsl:text>bo</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="form">
        <xsl:text>restored</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="odkazovane_heslo_rozepsane">
    <xsl:element name="xref">
      <xsl:attribute name="form">
        <xsl:text>restored</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="odkazovane_heslo_zkracene">
    <xsl:element name="xref">
      <xsl:attribute name="form">
        <xsl:text>short</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="text">
    <xsl:element name="text">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="Pismeno/text">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="text_delimitator">
    <xsl:element name="delim">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="text_tvar">
    <xsl:element name="morph">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="text_valence">
    <xsl:element name="val">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="vyznam">
    <xsl:element name="def">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="vyznam_delimitator">
    <xsl:element name="delim">
      <xsl:attribute name="rend">
        <xsl:text>it</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="zahnizdovane_heslo">
    <xsl:element name="hw">
      <xsl:attribute name="type">
        <xsl:text>subentry</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="zahnizdovane_heslo_odkazove">
    <xsl:element name="hw">
      <xsl:attribute name="type">
        <xsl:text>subentry</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="ref">
        <xsl:text>true</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="zahnizdovane_heslo_rozepsane">
    <xsl:element name="hw">
      <xsl:attribute name="type">
        <xsl:text>subentry</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="form">
        <xsl:text>restored</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="zahnizdovane_heslo_zkracene">
    <xsl:element name="hw">
      <xsl:attribute name="type">
        <xsl:text>subentry</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="form">
        <xsl:text>short</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
  <xsl:template match="zkratka_nonparej">
    <xsl:element name="abbr">
      <xsl:attribute name="rend">
        <xsl:text>nonp</xsl:text>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
