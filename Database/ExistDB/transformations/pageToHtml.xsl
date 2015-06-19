<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="2.0">
    <xsl:import href="commonToHtml-block.xsl"/>
    <xsl:import href="commonToHtml-inline.xsl"/>
    <xsl:strip-space elements="*"/>
    <xsl:template match="tei:w">
        <span data-nlp-type="{name()}" data-nlp-lemma="{@nlp:lemma}">

            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="result">
        <div class="itj-page">
            <xsl:apply-templates/>
        </div>
        <div class="itj-notes">
            <xsl:apply-templates mode="notes"/>
        </div>
    </xsl:template>
</xsl:stylesheet>