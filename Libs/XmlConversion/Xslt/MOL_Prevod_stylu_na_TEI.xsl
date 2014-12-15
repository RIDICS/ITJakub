<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
  exclude-result-prefixes="xd"
  version="1.0">
  <xd:doc scope="stylesheet">
    <xd:desc>
      <xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
      <xd:p><xd:b>Author:</xd:b> boris</xd:p>
      <xd:p>Převodník stylů specifický pro ediční modul VW</xd:p>
    </xd:desc>
  </xd:doc>
  <!--<xsl:include href="Prevod_stylu_na_TEI.xsl"/>-->
	
	<xsl:include href="Prevod_stylu_na_TEI.xsl"/>
	<xsl:include href="EM+EB_Prevod_stylu_na_TEI_spolecne.xsl"/>
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
	
  <xsl:template match="/">
  	<xsl:comment> MOL_Prevod_stylu_na_TEI </xsl:comment>
    <xsl:apply-templates />
  </xsl:template>
	
	<xsl:template match="comment()">
		<xsl:copy-of select="."/>
	</xsl:template>
  
	<xsl:template match="Bibliograficka_citace_originalu">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>biblio</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>annotation</xsl:text>
			</xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
  
	<xsl:template match="Autor">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>author</xsl:text>
			</xsl:attribute>
			<xsl:element name="author">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nadpis_1">
		<xsl:element name="head1">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="Nadpis_2">
		<xsl:element name="head2">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nadpis_3">
		<xsl:element name="head3">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Titul">
		<xsl:element name="title">
			<xsl:attribute name="type">
				<xsl:text>main</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Subtitle">
		<xsl:element name="title">
			<xsl:attribute name="type">
				<xsl:text>sub</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="internetovy_odkaz">
		<xsl:element name="ref">
			<xsl:attribute name="target"><xsl:apply-templates /></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Bibliograficky_popis">
		<xsl:element name="bibl">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="poznamka_pod_carou" priority="5">
		<xsl:element name="note">
			<xsl:attribute name="n">
				<xsl:value-of select="@n"/>
			</xsl:attribute>
			<xsl:attribute name="place">
				<xsl:text>bottom</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates select="footnote_text | Pozn._p._č."/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="footnote_text">
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="prolozene">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>interleaved</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="pagina">
		<xsl:call-template name="zpracujFoliaci">
			<xsl:with-param name="konciMezerou" select="substring(., string-length(.), 1) = ' '" />
			<xsl:with-param name="cislo" select="normalize-space(.)" />
		</xsl:call-template>
	</xsl:template>

	
<!--	<xd:doc>
		<xd:desc>Prvek <xd:b>pagina</xd:b> byl již převeden na prvek <xd:b>pb</xd:b> </xd:desc>
	</xd:doc>
	<xsl:template match="pb">
		<xsl:copy >
		<xsl:apply-templates select="@*" />
		</xsl:copy>
	</xsl:template>-->
	
	<xsl:template match="Resume">
		<xsl:element name="div">
			<xsl:attribute name="type"><xsl:text>resume</xsl:text></xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates />				
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Klicova_slova">
		<xsl:element name="div">
			<xsl:attribute name="type"><xsl:text>keywords</xsl:text></xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates />
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="tucne">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>bold</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="dolniIndex">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>sub</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="horni_index">
		<xsl:element name="hi">
			<xsl:attribute name="rend">
				<xsl:text>sup</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	
	<xd:doc>
		<xd:desc>Ignorované styly</xd:desc>
	</xd:doc>
	<xsl:template match="footnote_reference" />
</xsl:stylesheet>