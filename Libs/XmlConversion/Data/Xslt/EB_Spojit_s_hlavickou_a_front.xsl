<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xd" version="1.0" xmlns="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Vstupní soubor XML obsahuje prvek <xd:b>body</xd:b>.</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:include href="EB_Souhrnna_edicni_poznamka.xsl" />
	<xd:doc>
		<xd:desc>
			<xd:p>Hlavička obsahuje prvek <xd:b>teiHeader</xd:b>.</xd:p>
			<xd:p>Obvykle soubor <xd:i>_00.xml</xd:i>.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:param name="hlavicka" />

	<xd:doc>
		<xd:desc>
			<xd:p>Začátek obsahuje prvek <xd:b>front</xd:b>.</xd:p>
			<xd:p>Obvykle soubor <xd:i>_01.xml</xd:i>.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:param name="zacatek" />
	<xsl:output indent="yes" omit-xml-declaration="no" />
	<xsl:strip-space elements="*" />

	<xsl:template match="/">
		<xsl:comment> EB_Spojit_s_hlavickou_a_front </xsl:comment>
		<TEI xml:lang="cs" xmlns="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates mode="jmennyProstor" select="document($hlavicka)/*" />
			<text>
				<xsl:apply-templates mode="jmennyProstor" select="document($zacatek)/*" />
				<xsl:apply-templates mode="jmennyProstor" />
				<back>
					<!-- Anotace, která se objeví jako popiska/anotace elektronické knihy v e-shopu -->
					<xsl:apply-templates mode="jmennyProstorKonec" select="//div[@type='editorial' and @subtype='annotation']" />
					<!-- Souhrnná ediční poznámka určená pro všechny texty vydávané v reámci elektornických edic -->
					<xsl:call-template name="edicniPoznamka" />
					<!-- Upozornění na autorská práva, které se objeví při stahování elektronické knihy  -->
					<div subtype="copyright" type="editorial">
						<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
					</div>
				</back>
				<!--<xsl:call-template name="konec" />-->
			</text>
			<!-- Doplněno 18. 11. 2010 -->
		</TEI>
	</xsl:template>

	<xsl:template name="konec">
		<back>
			<xsl:call-template name="edicniPoznamka" />
			<!-- Anotace, která se objeví jako popiska/anotace elektronické knihy v e-shopu -->
			<!--<xsl:apply-templates select="//div[@type='editorial' and @subtype='annotation']" mode="jmennyProstorKonec"/>-->
			<!-- Upozornění na autorská práva, které se objeví při stahování elektronické knihy  -->
			<div subtype="copyright" type="editorial">
				<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
			</div>
		</back>
	</xsl:template>

	<xsl:template match="title[@type='commercial']" mode="jmennyProstor">
		<xsl:element name="head">
			<xsl:apply-templates mode="jmennyProstor" />
		</xsl:element>
	</xsl:template>
	<!--<xsl:template match="p[@type='grant']" mode="jmennyProstor" />-->


	<!--<xsl:template match="div[@type='editorial' and @subtype='annotation']" mode="jmennyProstor" />-->


	<xsl:template match="*" mode="jmennyProstorKonec">
		<xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates mode="jmennyProstor" select="@*|*|text()" />
		</xsl:element>
	</xsl:template>

	<xsl:template match="*" mode="jmennyProstor">
		<xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates mode="jmennyProstor" select="@*|*|text()" />
		</xsl:element>
	</xsl:template>

	<xsl:template match="@*" mode="jmennyProstor">
		<xsl:variable name="nazev">
			<xsl:choose>
				<xsl:when test="local-name() = 'id'">
					<xsl:text>xml:id</xsl:text>
				</xsl:when>
				<xsl:when test="local-name() = 'lang'">
					<xsl:text>xml:lang</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="local-name()" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="." />
		</xsl:attribute>
	</xsl:template>

</xsl:stylesheet>
