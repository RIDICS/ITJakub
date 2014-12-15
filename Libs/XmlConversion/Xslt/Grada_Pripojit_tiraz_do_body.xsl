<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Přejmenovává element <xd:b>head1</xd:b> na <xd:b>head</xd:b>. </xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:include href="Kopirovani_prvku.xsl"/>
	
	<xsl:template match="/">
		<xsl:comment> GRADA_Pripojit_tiraz_do_body </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	
	<xsl:template match="body">
		<xsl:element name="body">
			<xsl:call-template name="tiraz" />
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template name="tiraz">
		<!--<xsl:call-template name="tirazNabozenstvi" />-->
	</xsl:template>

	<xsl:template name="tirazNabozenstvi">
		<div type="editorial">
			<p>Publikace je výstupem VVZ MSM0021620841.</p>
		<p>prof. PhDr. Josef Kandert, CSc.</p>
		<p>NÁBOŽENSKÉ SYSTÉMY</p>
		<p>Člověk náboženský a jak mu porozumět</p>
		<p>Vydala Grada Publishing, a.s.</p>
		<p>U Průhonu 22, 170 00 Praha 7</p>
		<p>tel.: +420 234 264 401, fax: +420 234 264 400</p>
		<p>www.grada.cz</p>
		<p>jako svou 4199. publikaci</p>
		<p>Obrazový doprovod sestavil autor</p>
		<p>Odpovědná redaktorka PhDr. Pavla Landová</p>
		<p>Sazba a zlom Antonín Plicka</p>
		<p>Návrh a zpracování obálky Antonín Plicka</p>
		<p>Počet stran 200</p>
		<p>Vydání 1., 2010</p>
		<p>Vytiskly Tiskárny Havlíčkův Brod, a. s.</p>
		<p>© Grada Publishing, a.s., 2010</p>
		<p>ISBN 978-80-247-3166-7</p></div>
	</xsl:template>

</xsl:stylesheet>