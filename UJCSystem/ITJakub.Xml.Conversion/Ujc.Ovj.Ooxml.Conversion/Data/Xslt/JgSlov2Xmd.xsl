<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0" 
	xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
	exclude-result-prefixes="xs xd tei"
	version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jul 29, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
			<xd:p>Konverze aktuální verze JgSlov do metadat.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:output method="xml" indent="yes" />
	<xsl:variable name="version" select="tei:TEI/tei:teiHeader/tei:revisionDesc/tei:change[last()]/@n"/>
	<xsl:template match="/">
		<itj:document doctype="Dictionary" versionId="{$version}" xml:lang="cs" n="{/tei:TEI/tei:teiHeader/tei:fileDesc/@n}" xmlns="http://www.tei-c.org/ns/1.0" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:xml="http://www.w3.org/XML/1998/namespace">
			<xsl:apply-templates select="tei:TEI/tei:teiHeader" />
			<xsl:call-template name="itj-full-content" />
		</itj:document>
	</xsl:template>
	
	<xsl:template match="tei:teiHeader">
		<xsl:copy-of select="." />
	</xsl:template>

	<xsl:template name="itj-full-content">
		<xsl:call-template name="itj-pages" />
		<xsl:call-template name="itj-table-of-content" />
		<xsl:call-template name="itj-headwords-table" />
		<xsl:call-template name="itj-headwords-list" />
	</xsl:template>

	<xsl:template name="itj-pages">
		<itj:pages>
			<xsl:apply-templates select="//tei:surface" mode="pages" />
		</itj:pages>
	</xsl:template>

	<xsl:template match="tei:surface" mode="pages">
		<itj:page n="{@n}" xml:id="t-1.body-1.pb-{@n}-{position()}" facs="{tei:graphic/@url}" >
<!--			<xsl:attribute name="resource">
				<xsl:value-of select=" concat(substring-before(tei:graphic/@url, '.'), '.xml')"/>
			</xsl:attribute>
-->
		</itj:page>		
	</xsl:template>

	<xsl:template name="itj-table-of-content">
		<itj:tableOfContent />
	</xsl:template>

	<xsl:template name="itj-headwords-table">
		<itj:headwordsTable>
			<xsl:apply-templates select="//tei:term" mode="headwords-table" />
		</itj:headwordsTable>
	</xsl:template>

	<xsl:template match="tei:surface/tei:desc/tei:term" mode="headwords-table">
		<!-- <term id="1e1d80d4-b3c9-48cf-9266-fa2e24126a0c" type="detail" subtype="B" international="Břehovatý" n="009104">Břehowatý</term> -->
		<!-- defaultHw="{.}" defaultHw-sorting="{.}"  -->
		<xsl:param name="id">
			<xsl:choose>
				<xsl:when test="@type='main'"><xsl:value-of select="@n"/></xsl:when>
				<xsl:otherwise>
					<xsl:choose>
						<xsl:when test="preceding-sibling::tei:term[@type='main'][1]">
							<xsl:value-of select="preceding-sibling::tei:term[@type='main'][1]/@n"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="preceding::tei:term[@type='main'][1]/@n"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:param>
		<itj:headword entryId="{$id}">
			
			<xsl:variable name="main-hw">
				<xsl:choose>
					<xsl:when test="@type='main'"><xsl:value-of select="."/></xsl:when>
					<xsl:otherwise>
						<xsl:choose>
							<xsl:when test="preceding-sibling::tei:term[@type='main'][1]">
								<xsl:value-of select="preceding-sibling::tei:term[@type='main'][1]"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="preceding::tei:term[@type='main'][1]"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			
			
			
			<xsl:variable name="hw">
				<xsl:value-of select="@international"/>
			</xsl:variable>
			
			
			<xsl:variable name="default-hw-translit">
				<xsl:value-of select="$main-hw/text()"/>
			</xsl:variable>
			
			<xsl:variable name="hw-translit">
				<xsl:value-of select="."/>
			</xsl:variable>

			<xsl:variable name="default-hw">
				<xsl:choose>
					<xsl:when test="@type='main'"><xsl:value-of select="@international"/></xsl:when>
					<xsl:otherwise>
						<xsl:choose>
							<xsl:when test="preceding-sibling::tei:term[@type='main'][1]/@international">
								<xsl:value-of select="preceding-sibling::tei:term[@type='main'][1]/@international"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="preceding::tei:term[@type='main'][1]/@international"/>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			
			
			<xsl:attribute name="defaultHw">
				<xsl:value-of select="$default-hw"/>
			</xsl:attribute>
			
			<xsl:attribute name="defaultHw-sorting">
				<xsl:call-template name="hw-for-sorting">
					<xsl:with-param name="hw" select="$default-hw" />
				</xsl:call-template>
			</xsl:attribute>
			
			<xsl:attribute name="defaultHw-transliterated">
				<xsl:value-of select="$default-hw-translit"/>
			</xsl:attribute>
			<xsl:attribute name="defaultHw-transliterated-sorting">
				<xsl:call-template name="hw-for-sorting">
					<xsl:with-param name="hw" select="$default-hw-translit" />
				</xsl:call-template>
			</xsl:attribute>
			

			<xsl:attribute name="hw">
				<xsl:call-template name="hw-for-sorting">
					<xsl:with-param name="hw" select="$hw" />
				</xsl:call-template>
			</xsl:attribute>
			
			<xsl:attribute name="hw-original">
				<xsl:value-of select="$hw"/>
			</xsl:attribute>

			<xsl:attribute name="hw-transliterated">
				<xsl:call-template name="hw-for-sorting">
					<xsl:with-param name="hw" select="$hw-translit" />
				</xsl:call-template>
			</xsl:attribute>
			
			<xsl:attribute name="hw-original-transliterated">
				<xsl:value-of select="$hw-translit"/>
			</xsl:attribute>
			
			<xsl:attribute name="facs">
				<xsl:value-of select="parent::tei:desc/following-sibling::tei:graphic/@url"/>
			</xsl:attribute>
			
		</itj:headword>
	</xsl:template>
	
	<xsl:template name="hw-for-sorting">
		<xsl:param name="hw" select="''" />
		<xsl:value-of select="replace($hw, '[^a-žA-Ž0-9]', '')"/>
	</xsl:template>
	


	<xsl:template name="itj-headwords-list">
		<itj:headwordsList />
	</xsl:template>
	
</xsl:stylesheet>