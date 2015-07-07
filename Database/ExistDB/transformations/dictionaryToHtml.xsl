<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exist="http://exist.sourceforge.net/NS/exist" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd tei nlp exist" version="2.0">
	<?itj-book-type Dictionary?>
	<?itj-output-format Html?>
    <xsl:import href="pageToHtml.xsl"/>
    <xsl:output method="html" indent="no"/>
    <xsl:strip-space elements="*"/>
    <xsl:template match="tei:TEI">
        <div class="result">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="exist:match">
        <span class="match">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:entryFree">
        <div class="entryFree">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="tei:orth[not(@xml:lang) and not(@type)]">
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="tei:orth">
        <span>
            <xsl:choose>
                <xsl:when test="@xml:lang">
                    <xsl:attribute name="lang">
                        <xsl:value-of select="@xml:lang"/>
                    </xsl:attribute>
                </xsl:when>
                <xsl:when test="@type">
                    <xsl:attribute name="data-orth-type">
                        <xsl:value-of select="@type"/>
                    </xsl:attribute>
                </xsl:when>
            </xsl:choose>
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:num">
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="tei:cit">
        <span lang="{@xml:lang}" data-orth-type="{@type}">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="context[tei:entryFree]">
        <xsl:apply-templates select="tei:entryFree"/>
    </xsl:template>
    <xsl:template match="tei:entryFree/tei:sense">
        <div class="sense">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="tei:sense/tei:sense">
        <div class="sense">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="tei:def">
        <span class="def">
            <xsl:if test="@rend">
                <xsl:attribute name="class">
                    <xsl:text>def </xsl:text>
                    <xsl:value-of select="@rend"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:sense/tei:num">
        <span class="sense-num">
            <xsl:apply-templates/>
        </span>
    </xsl:template>
    <xsl:template match="tei:entryFree/tei:note">
        <div class="note">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="tei:entryFree/tei:note[@type='response']" priority="10">
        <div class="response">
            <xsl:apply-templates/>
        </div>
    </xsl:template>
    <xsl:template match="*[@type='hidden']" priority="20"/>
    <xsl:template match="*[@rend='hidden']" priority="20"/>
</xsl:stylesheet>