<?xml version="1.0" encoding="utf-8"?>
<axsl:stylesheet xmlns:axsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
   <axsl:output indent="yes" method="xml" standalone="yes"/>
   <axsl:strip-space elements="*"/>
   <axsl:template match="/">
      <axsl:apply-templates/>
   </axsl:template>
   <xsl:template match="body">
      <dictionary>
         <axsl:attribute name="name">
            <axsl:text>ESSC</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </dictionary>
   </xsl:template>
		
   <xsl:template match="Normalni"/>
		
   <axsl:template match="Pismeno">
      <axsl:element name="milestone">
         <axsl:attribute name="type">
            <axsl:text>letter</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Odkazove_zahlavi">
      <axsl:element name="entryhead">
         <axsl:attribute name="type">
            <axsl:text>ref</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Vyloucene_heslo">
      <axsl:element name="entryhead">
         <axsl:attribute name="type">
            <axsl:text>excl</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="Vyznam_rimske">
      <axsl:element name="senseGrp">
         <axsl:attribute name="type">
            <axsl:text>number</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
   <axsl:template match="Vyznam_abecedni">
      <axsl:element name="senseGrp">
         <axsl:attribute name="type">
            <axsl:text>letter</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
		
   <axsl:template match="Heslove_zahlavi">
      <axsl:element name="entryhead">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Vyklad_vyznamu">
      <axsl:element name="sense">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Vyklad_podvyznamu">
      <axsl:element name="sense">
         <axsl:attribute name="type">
            <axsl:text>subsense</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Poznamka">
      <axsl:element name="note">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Autor">
      <axsl:element name="resp">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Motivace">
      <axsl:element name="motivSect">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Delimitator_hesel">
      <axsl:element name="entryend">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="heslove_slovo">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_netucne">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>norm</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
   <axsl:template match="pramen">
      <axsl:element name="refsource">
         <axsl:attribute name="abbr">
            <axsl:value-of select="normalize-space(.)"/>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="komentar_pramen">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="abbr">
            <axsl:value-of select="normalize-space(.)"/>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="identifikator">
      <axsl:element name="identity">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="komentar">
      <axsl:element name="comment">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="komentar_kurziva">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="komentar_kurziva_preskrtnute">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it strikethrough</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
   <axsl:template match="komentar_kurziva_podtrzene">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it underline</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="komentar_kurziva_tucne">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="komentar_podtrzene">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>underline</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="komentar_preskrtnute">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>strike-trough</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>	
		
		
   <axsl:template match="komentar_tucne">
      <axsl:element name="text">
         <axsl:attribute name="type">
            <axsl:text>hidden</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="kurziva">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="morfologicka_charakteristika">
      <axsl:element name="morph">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="slovni_druh">
      <axsl:element name="pos">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="nonparej">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>nonp</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="zkratka">
      <axsl:element name="abbr">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="zkratka_kurziva">
      <axsl:element name="abbr">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="zkratka_nonparej">
      <axsl:element name="abbr">
         <axsl:attribute name="rend">
            <axsl:text>nonp</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
   <axsl:template match="odkaz">
      <axsl:element name="xref">
         <axsl:attribute name="type">
            <axsl:text>cf</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="realizace_hesloveho_slova">
      <axsl:element name="hw">
         <axsl:attribute name="type">
            <axsl:text>substandard</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="spojitelnost">
      <axsl:element name="val">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="text">
      <axsl:element name="text">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <xsl:template match="Autor/text">
      <axsl:apply-templates/>
   </xsl:template>
		
   <xsl:template match="Autor/text">
      <axsl:apply-templates/>
   </xsl:template>
		
   <xsl:template match="Pismeno/text">
      <axsl:apply-templates/>
   </xsl:template>
		
   <axsl:template match="ustalene_spojeni">
      <axsl:element name="hwcolloc">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		 
		
   <axsl:template match="ustalene_spojeni_zkracene">
      <axsl:element name="hwcolloc">
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="ustalene_spojeni_rozepsane">
      <axsl:element name="hwcolloc">
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="ustalene_spojeni_rozepsane_delimitator">
      <axsl:element name="delim">
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="vyznam">
      <axsl:element name="def">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="vyznam_starocesky_vyraz">
      <axsl:element name="def">
         <axsl:attribute name="lang">
            <axsl:text>oldcze</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="akce">
      <axsl:element name="action">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="delimitator">
      <axsl:element name="delim">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="delimitator_tucne">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="delimitator_vyznamu">
      <axsl:element name="delim">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="*">
      <axsl:copy>
         <axsl:copy-of select="@*"/>
         <axsl:apply-templates/>
      </axsl:copy>
   </axsl:template>
		
		
   <xsl:template match="annotation_reference"/>

	
</axsl:stylesheet>