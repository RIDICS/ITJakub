<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
    xmlns="http://www.tei-c.org/ns/1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	exclude-result-prefixes="xd" version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>

    <xsl:template match="/">
<xsl:text xml:space="preserve">
</xsl:text>
        <xsl:comment> Vokab1550_Prevod-Stylu </xsl:comment>
<xsl:text xml:space="preserve">
</xsl:text>
        <xsl:element name="body">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="Titul">
        <xsl:element name="head0">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="Nadpis">
        <xsl:element name="head1">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="Podnadpis">
        <xsl:element name="head2">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="Nadpis/cestina | Normalni[not(latina)]/cestina">
        <xsl:element name="foreign">
            <xsl:attribute name="xml:lang">
                <xsl:text>cs-x-translit</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>

    <xsl:template match="Nadpis/cestina_transkripce | Normalni[not(latina)]/cestina_transkripce">
        <xsl:element name="foreign">
            <xsl:attribute name="xml:lang">
                <xsl:text>cs-x-transcr</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="kurziva">
        <xsl:element name="hi">
            <xsl:attribute name="rend">
                <xsl:text>it</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>

    <xsl:template match="Podnadpis/latina | Nadpis/latina">
        <xsl:element name="foreign">
            <xsl:attribute name="xml:lang">
                <xsl:text>la</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="Normalni[latina]">
        <xsl:element name="entryFree">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="Normalni[not(latina)]">
        <xsl:element name="p">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="paginace">
        <xsl:if test="string-length(normalize-space(.)) &gt; 0">
        <xsl:element name="pb">
            <xsl:attribute name="n">
                <xsl:value-of select="normalize-space(translate(., '[]', ''))"/>
            </xsl:attribute>
            <xsl:if test="string-length(translate(., '[]', '')) &gt; string-length(normalize-space(translate(., '[]', '')))">
                <xsl:attribute name="rend"><xsl:text>space</xsl:text></xsl:attribute>
            </xsl:if>
        </xsl:element>
        </xsl:if>
    </xsl:template>

    <xsl:template match="delimitator_jazyka">
        <xsl:element name="hi">
            <xsl:attribute name="type">
                <xsl:text>delimitator</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="cestina">
        <xsl:element name="cit">
            <xsl:element name="orth">
                <xsl:attribute name="type">
                    <xsl:text>transliteration</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="xml:lang">
                    <xsl:text>cs-x-translit</xsl:text>
                </xsl:attribute>
                <xsl:apply-templates/>
            </xsl:element>
        </xsl:element>
    </xsl:template>

    <xsl:template match="cestina_transkripce">
        <xsl:element name="cit">
            <xsl:element name="orth">
                <xsl:attribute name="type">
                    <xsl:text>transcription</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="xml:lang">
                    <xsl:text>cs-x-transcr</xsl:text>
                </xsl:attribute>
                <xsl:apply-templates/>
            </xsl:element>
        </xsl:element>
    </xsl:template>

    <xsl:template match="latina">
        <xsl:element name="form">
            <xsl:element name="orth">
                <xsl:attribute name="xml:lang">
                    <xsl:text>la</xsl:text>
                </xsl:attribute>
                <xsl:apply-templates/>
            </xsl:element>
        </xsl:element>
     </xsl:template>

    <xsl:template match="nemcina">
        <xsl:call-template name="vytvorEkvivalent">
            <xsl:with-param name="jazyk">
                <xsl:text>de</xsl:text>
            </xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="vytvorEkvivalent">
        <xsl:param name="jazyk"/>
        <xsl:element name="cit">
            <xsl:attribute name="type">
                <xsl:text>translation</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="xml:lang">
                <xsl:value-of select="$jazyk"/>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="poznamka_pod_carou">
        <xsl:element name="note">
            <xsl:attribute name="n">
                <xsl:value-of select="@n"/>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xsl:template match="footnote_text">
        <p>
            <xsl:apply-templates/>
        </p>
    </xsl:template>

    <xsl:template match="footnote_text/kurziva">
        <foreign xml:lang="lat" rend="it">
            <xsl:apply-templates/>
            <xsl:if test="following-sibling::*[1]/self::text">
                <xsl:if test="following-sibling::*[1]/self::text/text() = ' '">
                    <xsl:value-of select="following-sibling::*[1]/self::text/text()"/>
                </xsl:if>
            </xsl:if>
        </foreign>
    </xsl:template>

    <!-- Opravit ve zdrojových datech - označit jazyk v poznámkách -->
    <xsl:template match="footnote_text/text">
        <!-- <xsl:attribute name="xml:lang"><xsl:text>grc</xsl:text></xsl:attribute>-->
        <xsl:apply-templates/>
    </xsl:template>

    <xsl:template match="text()">
        <xsl:variable name="text" select="translate(., '&#009;', ' ')"/>
        <xsl:if test="substring($text, string-length($text), 1) = ' '">
            <xsl:attribute name="xml:space">
                <xsl:value-of select="'preserve'"/>
            </xsl:attribute>
        </xsl:if>
        <xsl:value-of select="$text"/>
    </xsl:template>
    
    <xsl:template match="Hyperlink">
        <xsl:choose>
            <xsl:when test="starts-with(., 'http:')">
                <xsl:element name="ref">
                    <xsl:attribute name="target"><xsl:value-of select="."/></xsl:attribute>
                    <xsl:apply-templates />
                </xsl:element>
            </xsl:when>
            <xsl:otherwise>
                <xsl:apply-templates />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:stylesheet>
