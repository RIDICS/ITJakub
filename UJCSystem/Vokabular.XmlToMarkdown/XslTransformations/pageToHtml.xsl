<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist" exclude-result-prefixes="xd tei nlp exist itj vw" version="2.0">
	<?itj-book-type Edition?>
	<?itj-output-format Html?>
    <xsl:import href="commonToHtml-block.xsl"/>
    <xsl:import href="commonToHtml-inline.xsl"/>
    <xsl:output method="html" encoding="UTF-8"/>
    <xsl:strip-space elements="*"/>
    <xsl:output indent="yes"/>
    <xsl:variable name="book-type" select="''"/>
    <xsl:template match="tei:l">
        <span class="itj-line" data-nlp-type="{name()}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:w">
        <span data-nlp-type="{name()}" data-nlp-lemma="{@nlp:lemma}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="exist:match">
        <span class="reader-search-result-match">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="result | vw:fragment">
        <div class="itj-page">
            <xsl:apply-templates/>
        </div>        
    </xsl:template>
    <xsl:template match="tei:div[@type= 'editorial']//tei:w">
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="tei:div[@type= 'editorial']//tei:pc">
        <xsl:apply-templates/>
    </xsl:template>
</xsl:stylesheet>