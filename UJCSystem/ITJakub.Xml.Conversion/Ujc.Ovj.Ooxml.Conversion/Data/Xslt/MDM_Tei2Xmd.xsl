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
			<xd:p><xd:b>Created on:</xd:b> Aug 20, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
		<xsl:output method="xml" indent="yes" encoding="UTF-8" />
	
	<xsl:template match="/">
		<itj:document doctype="Grammar" versionId="02cfe5fd-9045-4b7e-a6a8-5255fe2c0f1f" xml:lang="cs" n="{/tei:TEI/tei:teiHeader/tei:fileDesc/@n}" 
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
				<bibl type="acronym" subtype="source"><xsl:value-of select="tei:TEI/tei:teiHeader/@n"/></bibl>
			</listBibl>
		</sourceDesc>
	</xsl:template>
	
	
	<xsl:template match="tei:fileDesc">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:apply-templates />
		</xsl:copy>
		
		<encodingDesc xmlns="http://www.tei-c.org/ns/1.0">
			<classDecl>
				<taxonomy xml:id="taxonomy">
					<category xml:id="taxonomy-dictionary">
						<catDesc xml:lang="cs-cz">slovník</catDesc>
						<category xml:id="taxonomy-dictionary-contemporary">
							<catDesc xml:lang="cs-cz">soudobý</catDesc>
						</category>
						<category xml:id="taxonomy-dictionary-historical">
							<catDesc xml:lang="cs-cz">dobový</catDesc>
						</category>
					</category>
					<category xml:id="taxonomy-historical_text">
						<catDesc xml:lang="cs-cz">historický text</catDesc>
						<category xml:id="taxonomy-historical_text-old_czech">
							<catDesc xml:lang="cs-cz">staročeský</catDesc>
						</category>
						<category xml:id="taxonomy-historical_text-medieval_czech">
							<catDesc xml:lang="cs-cz">středněčeský</catDesc>
						</category>
					</category>
					<category xml:id="taxonomy-scholary_text">
						<catDesc xml:lang="cs-cz">odborný text</catDesc>
					</category>
					<category xml:id="taxonomy-digitized-grammar">
						<catDesc xml:lang="cs-cz">digitalizovaná mluvnice</catDesc>
					</category>
					<category xml:id="taxonomy-card-index">
						<catDesc xml:lang="cs-cz">lístková kartotéka</catDesc>
					</category>
				</taxonomy>
				<taxonomy xml:id="output">
					<category xml:id="output-dictionary">
						<catDesc xml:lang="cs-cz">slovníky</catDesc>
					</category>
					<category xml:id="output-editions">
						<catDesc xml:lang="cs-cz">edice</catDesc>
					</category>
					<category xml:id="output-text_bank">
						<catDesc xml:lang="cs-cz">textová banka</catDesc>
						<category xml:id="output-text_bank-old_czech">
							<catDesc xml:lang="cs-cz">staročeská textová banka</catDesc>
						</category>
						<category xml:id="output-text_bank-middle_czech">
							<catDesc xml:lang="cs-cz">středněčeská textová banka</catDesc>
						</category>
					</category>
					<category xml:id="output-scholary_literature">
						<catDesc xml:lang="cs-cz">odborná literatura</catDesc>
					</category>
					<category xml:id="output-digitized-grammar">
						<catDesc xml:lang="cs-cz">digitalizované mluvnice</catDesc>
					</category>
				</taxonomy>
			</classDecl>
		</encodingDesc>
		
		<revisionDesc xmlns="http://www.tei-c.org/ns/1.0">
			<change n="02cfe5fd-9045-4b7e-a6a8-5255fe2c0f1f" when="2015-07-22T08:57:36.3106449Z"></change>
		</revisionDesc>
		
	</xsl:template>
	
	<xsl:template match="tei:profileDesc">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:apply-templates />
			<textClass xmlns="http://www.tei-c.org/ns/1.0">
				<catRef target="#taxonomy-digitized-grammar #output-digitized-grammar" />
				</textClass>
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