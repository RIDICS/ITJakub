<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xs xd t" version="2.0" xmlns:t="http://www.tei-c.org/ns/1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 19, 2012</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p />
        </xd:desc>
    </xd:doc>

    <xsl:output encoding="UTF-8" indent="no" method="xml" omit-xml-declaration="no" />
    <xsl:strip-space elements="*" />
    <xsl:include href="Kopirovani_prvku.xsl" />

    <xsl:template match="/">
        <xsl:comment> EB_Vybor_Preskupit_pasaze_na_zaver </xsl:comment>
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="t:TEI">
        <xsl:copy>
            <xsl:apply-templates select="@*" />
            <xsl:apply-templates select="t:teiHeader" />
            <xsl:apply-templates select="t:text" />
        </xsl:copy>
    </xsl:template>

    <xsl:template match="t:text">
        <xsl:copy>
            <xsl:apply-templates select="t:front" />
            <xsl:apply-templates select="t:body" />
            <xsl:apply-templates select="t:back" />
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="t:titlePage">
        <xsl:copy>
            <xsl:apply-templates />
        </xsl:copy>
        <xsl:apply-templates select="/t:TEI/t:text/t:body/t:div[@subtype='annotation' and @type='editorial']" mode="tiraz" />
    </xsl:template>

    <xsl:template match="t:div[@subtype='imprint' and @type='editorial']">
        <xsl:apply-templates mode="tiraz" select="//t:div[@subtype='title' and @type='editorial']" />
        <div subtype="imprint" type="editorial" xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:attribute name="xml:id">
                <xsl:value-of select="generate-id()" />
            </xsl:attribute>
            <xsl:apply-templates select="t:p[position() &lt; 8]" />
            <xsl:apply-templates mode="tiraz" select="//t:p[@rend='grant']" />
            <xsl:apply-templates select="t:p[position() &gt; 7]" />
        </div>
        <xsl:call-template name="loga" />
        
    </xsl:template>

    <xsl:template match="t:body/t:div[1 and not(t:head)]">
        <xsl:copy>
            <xsl:apply-templates select="@*" />
            <head xmlns="http://www.tei-c.org/ns/1.0">
                <xsl:apply-templates select="/t:TEI/t:teiHeader/t:fileDesc/t:titleStmt/t:title/node()" />
            </head>
            <xsl:apply-templates />
        </xsl:copy>
    </xsl:template>


    <xsl:template name="vydal">
        <p xmlns="http://www.tei-c.org/ns/1.0" />
        <p xmlns="http://www.tei-c.org/ns/1.0">Vydal Ústav pro jazyk český AV ČR, v. v. i., v Nakladatelství Academia.</p>
        <p xmlns="http://www.tei-c.org/ns/1.0"><xsl:value-of select="/t:TEI/t:teiHeader/t:fileDesc/t:publicationStmt/t:pubPlace" /><xsl:text> </xsl:text><xsl:value-of select="/t:TEI/t:teiHeader/t:fileDesc/t:publicationStmt/t:date" />.</p>
    </xsl:template>

    <xsl:template name="loga">
        <div subtype="loga" type="editorial" xmlns="http://www.tei-c.org/ns/1.0">
            <figure subtype="A4">
                <graphic url="Logo_ujc.png" />
            </figure>
            <figure subtype="A6">
                <graphic url="Logo_ujc_A6.png" />
            </figure>
            <figure subtype="A4">
                <graphic url="Logo_academia.png" />
            </figure>
            <figure subtype="A6">
                <graphic url="Logo_academia_A6.png" />
            </figure>
        </div>
        <div subtype="edited" type="editorial" xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:call-template name="vydal" />
        </div>
    </xsl:template>

    <xsl:template match="t:p[@rend='grant']" mode="tiraz">
        <p xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:apply-templates />
        </p>
        <p xmlns="http://www.tei-c.org/ns/1.0" />
    </xsl:template>
    
    <xsl:template match="t:div[@subtype='annotation' and @type='editorial']" mode="tiraz">
        <xsl:copy>
            <xsl:apply-templates select="@*" />
            <xsl:apply-templates select="t:p" />
        </xsl:copy>
    </xsl:template>
    
    <!--<xsl:template match="t:div[@subtype='annotation' and @type='editorial']" />-->

    <xsl:template match="t:div[@subtype='title' and @type='editorial'][t:p]" mode="tiraz">
        <div subtype="annotation" type="editorial" xmlns="http://www.tei-c.org/ns/1.0">
            <xsl:apply-templates select="t:p" />
        </div>
    </xsl:template>

    <xsl:template match="t:div[@subtype='title' and @type='editorial']" />

    <xsl:template match="t:p[@rend='grant']" />

    <xsl:template match="t:body/t:div[@subtype='comment' and @type='editorial']" mode="back">
        <xsl:copy>
            <xsl:apply-templates select="@*" />
            <xsl:apply-templates />
        </xsl:copy>
    </xsl:template>

    <xsl:template match="t:body/t:div[@subtype='comment' and @type='editorial']" />

    <xsl:template match="t:back">
        <xsl:copy>
            <xsl:apply-templates select="t:div[@subtype='comment' and @type='editorial']" />
            <xsl:apply-templates mode="back" select="//t:body/t:div[@subtype='comment' and @type='editorial']" />
            <xsl:apply-templates select="t:div[@subtype='annotation' and @type='editorial']" />
            <xsl:apply-templates select="t:div[@subtype='copyright' and @type='editorial']" />
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
