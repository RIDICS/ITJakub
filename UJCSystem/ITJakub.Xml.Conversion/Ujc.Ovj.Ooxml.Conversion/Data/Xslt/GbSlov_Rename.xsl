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
            <xsl:text>GbSlov</xsl:text>
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
   <xsl:template match="Heslova_stat">
      <xsl:element name="entry">
         <xsl:attribute name="type">
            <xsl:text>full</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="Odkazova_stat">
      <xsl:element name="entry">
         <xsl:attribute name="type">
            <xsl:text>ref</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="Zahlavi_strany">
      <xsl:apply-templates/>
   </xsl:template>
   <xsl:template match="Zahlavi_strany/cislo_strany">
      <xsl:element name="pb">
         <xsl:attribute name="n">
            <xsl:value-of select="normalize-space(.)"/>
         </xsl:attribute>
      </xsl:element>
   </xsl:template>
   <xsl:template match="cislo_strany">
      <xsl:element name="pb">
         <xsl:attribute name="n">
            <xsl:value-of select="normalize-space(.)"/>
         </xsl:attribute>
      </xsl:element>
   </xsl:template>
   <xsl:template match="biblicke_misto">
      <xsl:element name="refsource">
         <xsl:attribute name="type">
            <xsl:text>bible</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="cislo_homonyma">
      <xsl:element name="hom">
         <xsl:attribute name="id">
            <xsl:value-of select="normalize-space(.)"/>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="delimitator">
      <xsl:element name="delim">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="delimitator_kurziva">
      <xsl:element name="delim">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="delimitator_tucne_zkracene">
      <xsl:element name="delim">
         <xsl:attribute name="rend">
            <xsl:text>bo</xsl:text>
         </xsl:attribute>
         <xsl:attribute name="form">
            <xsl:text>restored</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="delimitator_tucne">
      <xsl:element name="delim">
         <xsl:attribute name="rend">
            <xsl:text>bo</xsl:text>
         </xsl:attribute>
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
   <xsl:template match="heslove_slovo_rozepsane_delimitator">
      <xsl:element name="delim">
         <xsl:attribute name="rend">
            <xsl:text>bo</xsl:text>
         </xsl:attribute>
         <xsl:attribute name="form">
            <xsl:text>restored</xsl:text>
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
   <xsl:template match="horni_index">
      <xsl:element name="text">
         <xsl:attribute name="rend">
            <xsl:text>sup</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="charakteristika">
      <xsl:element name="gloss">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="charakteristika_kurziva">
      <xsl:element name="gloss">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="kurziva">
      <xsl:element name="text">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="lokace">
      <xsl:element name="location">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="morfologicka_charakteristika">
      <xsl:element name="morph">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="odkaz">
      <xsl:element name="xref">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="odkaz_tucne">
      <xsl:element name="xref">
         <xsl:attribute name="rend">
            <xsl:text>bo</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="odkaz_kurziva">
      <xsl:element name="xref">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="podhesli">
      <xsl:element name="hw">
         <xsl:attribute name="rend">
            <xsl:text>snserif</xsl:text>
         </xsl:attribute>
         <xsl:attribute name="type">
            <xsl:text>subentry</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="podhesli_rozepsane">
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
   <xsl:template match="podhesli_zkracene">
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
   <xsl:template match="podhesli_delimitator">
      <xsl:element name="delim">
         <xsl:attribute name="rend">
            <xsl:text>snserif</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="pramen">
      <xsl:element name="refsource">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="pramen_kurziva">
      <xsl:element name="refsource">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="prolozene">
      <xsl:element name="text">
         <xsl:attribute name="rend">
            <xsl:text>spaced</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="slovni_druh">
      <xsl:element name="pos">
         <xsl:attribute name="rend">
            <xsl:text>norm</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="slovni_druh_kurziva">
      <xsl:element name="pos">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
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
   <xsl:template match="tucne">
      <xsl:element name="text">
         <xsl:attribute name="rend">
            <xsl:text>bo</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="vyznam">
      <xsl:element name="def">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="zkratka">
      <xsl:element name="abbr">
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
   <xsl:template match="zkratka_kurziva">
      <xsl:element name="abbr">
         <xsl:attribute name="rend">
            <xsl:text>it</xsl:text>
         </xsl:attribute>
         <xsl:apply-templates/>
      </xsl:element>
   </xsl:template>
</xsl:stylesheet>
