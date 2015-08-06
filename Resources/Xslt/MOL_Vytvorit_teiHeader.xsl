<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xml="http://www.w3.org/XML/1998/namespace" xmlns:b="#default" exclude-result-prefixes="xd b" version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Mar 21, 2013</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p>Vytvoří hlavičku přesunem existujících dat</xd:p>
		</xd:desc>
	</xd:doc>


	<xsl:output indent="yes"/>

	<xsl:strip-space elements="*"/>

	<xsl:include href="Kopirovani_prvku.xsl"/>
	<xsl:include href="TEI_Common.xsl"/>

	<xsl:template match="/">
		<xsl:comment> MOL_Vytvorit_teiHeader </xsl:comment>
		<TEI xmlns="http://www.tei-c.org/ns/1.0">
			<xsl:element name="teiHeader">
				<fileDesc>
					<titleStmt>
						<xsl:apply-templates select="body//title[@type='main']" mode="teiHeader"/>
						<xsl:apply-templates select="body//title[@type='sub']" mode="teiHeader"/>
						<xsl:apply-templates select="body//div[@type='author']/author" mode="teiHeader"/>

						<!--<xsl:copy-of select="body//title[@type='main']" copy-namespaces="no" />
                    <xsl:copy-of select="body//title[@type='sub']" copy-namespaces="no" />
                    <xsl:copy-of select="body//div[@type='author']/author" copy-namespaces="no" />-->

						<!--<title><xsl:apply-templates select="body//title[@type='main']"/></title>
                    <xsl:apply-templates select="body//title[@type='main']"/>
                    <xsl:apply-templates select="body//title[@type='sub']"/>
                    <author><xsl:apply-templates select="body//div[@type='author']/author"/></author>-->
						<respStmt>
							<resp>kolace</resp>
							<name>Šmejkalová, Michaela</name>
						</respStmt>
						<respStmt>
							<resp>kódování TEI</resp>
							<name>Lehečka, Boris</name>
						</respStmt>
					</titleStmt>
					<publicationStmt>
						<publisher>oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.<email>vyvoj@ujc.cas.cz</email></publisher>
						<pubPlace>Praha</pubPlace>
						<date>2013</date>
						<availability status="restricted">
							<p>Tato elektronická edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p>
						</availability>
					</publicationStmt>
					<!--<publicationStmt>
                    <publisher>Univerzita J. E. Purkyně</publisher>
                    <pubPlace>Ústí nad Labem</pubPlace>
                    <date>2011</date>
                </publicationStmt>-->
					<sourceDesc>
						<bibl>
							<xsl:apply-templates select="body//div[@type='biblio' and @subtype='annotation']/p" mode="content"/>
						</bibl>
					</sourceDesc>
				</fileDesc>
				<xsl:call-template name="InsertEndocingDesc"/>
				<profileDesc>
					<textClass>
						<catRef target="#taxonomy-scholary_text"/>
						<xsl:if test="body//div[@type='keywords']/p[@xml:lang='cs']">
							<keywords xml:lang="cs">
								<xsl:apply-templates select="body//div[@type='keywords']/p[@xml:lang='cs']/text()" mode="teiHeader"/>
							</keywords>
						</xsl:if>
						<xsl:if test="body//div[@type='keywords']/p[@xml:lang='en']">
							<keywords xml:lang="en">
								<xsl:apply-templates select="body//div[@type='keywords']/p[@xml:lang='en']/text()" mode="teiHeader"/>
							</keywords>
						</xsl:if>
					</textClass>
				</profileDesc>
			</xsl:element>
			<xsl:element name="text">
				<front>
					<docTitle>
						<titlePart type="main">
							<xsl:apply-templates select="body//title[@type='main']" mode="content"/>
						</titlePart>
						<xsl:if test="body//title[@type='sub']">
							<titlePart type="sub">
								<xsl:apply-templates select="body//title[@type='sub']" mode="content"/>
							</titlePart>
						</xsl:if>
					</docTitle>
					<docAuthor>
						<xsl:apply-templates select="body//div[@type='author']/author" mode="content"/>
					</docAuthor>
					<div>
						<bibl>
							<xsl:apply-templates select="body//div[@type='biblio' and @subtype='annotation']/p" mode="content"/>
						</bibl>
					</div>
				</front>
				<xsl:apply-templates select="body"/>
			</xsl:element>
		</TEI>
	</xsl:template>


	<xsl:template match="body//title[@type='main']" priority="20"/>
	<xsl:template match="body//title[@type='sub']" priority="20"/>
	<xsl:template match="body//div[@type='author']" priority="20"/>
	<xsl:template match="body//div[@type='biblio' and @subtype='annotation']" priority="20"/>
	<xsl:template match="p[not(node())]" priority="20"/>

	<xsl:template match="*" xmlns="#defult" priority="10">
		<xsl:element name="{name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="*" xmlns="#defult" priority="10" mode="teiHeader">
		<xsl:element name="{name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates mode="teiHeader"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="div[@type='keywords']/p/text()" name="tokenize" mode="teiHeader" priority="10">
		<xsl:param name="separator" select="','"/>
		<xsl:for-each select="tokenize(.,$separator)">
			<term xmlns="http://www.tei-c.org/ns/1.0">
				<xsl:value-of select="normalize-space(.)"/>
			</term>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="body//div[@type='biblio' and @subtype='annotation']/p" mode="content">
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template match="body//div[@type='author']/author" mode="content">
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template match="title[@type]" mode="content">
		<xsl:apply-templates/>
	</xsl:template>

	<xsl:template match="note" mode="teiHeader" priority="30"/>
</xsl:stylesheet>
