<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist" exclude-result-prefixes="xd tei nlp exist itj vw" version="2.0">
  <?itj-book-type Edition?>
  <?itj-output-format Text?>
  <xsl:output method="text" encoding="UTF-8"/>
  <xsl:strip-space elements="*"/>
  <xsl:output indent="yes"/>
  <xsl:variable name="book-type" select="''"/>
  <xsl:template match="tei:l">
    <xsl:apply-templates/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xsl:template match="tei:w">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="exist:match">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="result | vw:fragment">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="tei:div[@type= 'editorial']//tei:w">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="tei:div[@type= 'editorial']//tei:pc">
    <xsl:apply-templates/>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro původní titul díla, uvedený v prameni.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:title">
    <xsl:text>&#xa;## </xsl:text>
    <xsl:apply-templates/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro nadpis.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:head">
    <xsl:text>&#xa;### </xsl:text>
    <xsl:apply-templates/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro ediční komentář.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:div[@type='editorial' and @subtype='comment']">
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro informaci o grantové podpoře.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:div[@type='editorial' and @subtype='grant']">
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro incipit a explicit.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:div[@type='incipit' or @type='explicit']">

  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro biblickou knihu.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:div[@type='bible' and @subtype='book']">

  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro biblickou kapitolu</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:div[@type='bible' and @subtype='chapter']">

  </xsl:template>
  <xsl:template match="tei:p">
    <xsl:text>&#xa;</xsl:text>
    <xsl:apply-templates/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro seznam typu rejstřík.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:list[@type='index']">

  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro tabulku</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:table">

  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro řádek v tabulce</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:row">

  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro buňku v tabulce</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:cell">

  </xsl:template>
  <xsl:template match="tei:note[@n]" mode="notes">

  </xsl:template>
  <xd:doc>
    <xd:desc>Šablona pro interpunkci.</xd:desc>
  </xd:doc>
  <xsl:template match="tei:pc">
    <xsl:apply-templates/>
  </xsl:template>
  <xd:doc>
    <xd:desc>Šablona pro jednoznakovou mezeru.</xd:desc>
  </xd:doc>
  <xsl:template match="tei:c[@type='space']">
    <xsl:text> </xsl:text>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro zvýraznění textu.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:hi">
    <xsl:text>**</xsl:text>
    <xsl:apply-templates/>
    <xsl:text>**</xsl:text>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro kapitálku (obvykle první písmeno v knize, kapitole nebo odstavci).</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:c">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="tei:note[@n]">
  </xsl:template>
  <xsl:template match="tei:corr" mode="notes">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="tei:sic" mode="notes">
    <span class="sic">
      <xsl:apply-templates/>
    </span>
  </xsl:template>
  <xd:doc>
    <xd:desc>
      <xd:p>Šablona pro hranice stran originálního textu.</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:template match="tei:pb">

  </xsl:template>
  <xsl:template match="tei:*[@rend]" priority="-1">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="tei:*[@rend='hidden']" priority="10">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="tei:*[@type='hidden']" priority="10">
    <xsl:apply-templates/>
  </xsl:template>
</xsl:stylesheet>