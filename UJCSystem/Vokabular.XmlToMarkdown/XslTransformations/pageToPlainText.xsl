<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:vw="http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0/exist" exclude-result-prefixes="xd tei nlp exist itj vw" version="2.0">
	<?itj-book-type Edition?>
	<?itj-output-format Text?>
    <xsl:import href="commonToText-block.xsl"/>
    <xsl:import href="commonToText-inline.xsl"/>
    <xsl:output method="text" encoding="UTF-8"/>
    <xsl:strip-space elements="*"/>
    <xsl:output indent="yes"/>
    <xsl:variable name="book-type" select="''"/>
    <xsl:template match="tei:l">
        <xsl:apply-templates/><xsl:text>&#xa;</xsl:text>
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
</xsl:stylesheet>