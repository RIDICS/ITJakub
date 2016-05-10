<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="xd saxon"
    version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p></xd:p>
        </xd:desc>
    </xd:doc>
    
    <xsl:strip-space elements="*"/>
    <xsl:preserve-space elements="tei:entryFree"/>
    
    <xsl:output method="html"
        doctype-system="http://www.w3.org/TR/html4/strict.dtd" 
        doctype-public="-//W3C//DTD HTML 4.01//EN"
        indent="yes" />
    
    <!--<xsl:param name="titul" />-->
    
    <xsl:template match="/">
        <html>
            <head>
                <!--<title><xsl:value-of select="$titul"/></title>-->
                <title><xsl:value-of select="'slovník'"/></title>
                <style type="text/css">
                    .tlacitko
                    {
                    text-decoration: none;
                    font-weight: bold;
                    background-color: #dcdcdc;
                    color: #000000;
                    font-size: smaller;
                    padding-right: 4px;
                    padding-left: 4px;
                    padding-bottom: 2px;
                    padding-top: 2px;
                    position: relative;
                    top: 4px;
                    margin-left: 4px;
                    margin-right: 4px;
                    border-right: thin outset;
                    border-top: thin outset;
                    border-left: thin outset;
                    border-bottom: thin outset;
                    } 
                    body {background-color: #fae6d7;}
                    h1, h2, h3 {font-weight: bold; }
                    h1 {font-size: 135%; }
                    h2 {font-size: 120%;}
                    h3 {font-size: 110%;}
                    .it {font-style: italic;}
                    .la {font-weight: bold;}
                    .de {font-family: monospace;}
                    .supplied {color: DimGray; }
                    .cs-x-transcr {font-size: 80%; }
                    .cs-x-translit {}
                    .note-id {vertical-align: super; font-size: 75%;}
                    .note { padding: 0px; margin: 0px; line-height: 1em; color: LightSlateGrey;  }
                    .notes { font-size: 80%; display: block; padding-left: 20px; margin-top: 5px; line-height: 1em; }
                    .page { background-color: black; color: white; }
                    .entry {margin-top: 10px; margin-bottom: 0px;}
                    div.transcription {display: block;  /* border-radius: 15px; */
                    border-left: 2px solid LightSlateGrey; border-right: 2px solid LightSlateGrey; border-bottom: 2px solid LightSlateGrey;
                    padding: 1px; }
                </style>
            </head>
            <body>
                <xsl:apply-templates />
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="tei:body/tei:div">
        <div>
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="tei:body/tei:div/tei:head">
        <h1><xsl:apply-templates /></h1>
        <xsl:call-template name="create-transcription-div" />
        <xsl:call-template name="createNotesDiv"/>
    </xsl:template>
    
    <xsl:template match="tei:body/tei:div/tei:div/tei:head">
        <h2><xsl:apply-templates /></h2>
        <xsl:call-template name="create-transcription-div" />
        <xsl:call-template name="createNotesDiv"/>
    </xsl:template>
    
    <xsl:template match="tei:body/tei:div/tei:div/tei:div/tei:head">
        <h3><xsl:apply-templates /></h3>
        <xsl:call-template name="create-transcription-div" />
        <xsl:call-template name="createNotesDiv"/>
    </xsl:template>
    
    <xsl:template match="tei:note">
        <span class="note-id">
            <xsl:value-of select="@n"/>
        </span>
    </xsl:template>
    
    <xsl:template match="tei:note" mode="noteText" >
        <p class="note">
            <span class="note-id"><xsl:value-of select="@n"/></span>
            <xsl:apply-templates mode="noteText" />
        </p>
    </xsl:template>
    
    <xsl:template match="tei:p" mode="noteText" >
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="tei:form">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match=" tei:foreign | tei:cit[not(tei:orth)] | tei:orth | tei:orig">
        <span>
            <xsl:attribute name="class">
                <xsl:value-of select="@xml:lang"/>
                <xsl:if test="@rend">
                    <xsl:value-of select="concat(' ', @rend)"/>   
                </xsl:if>
            </xsl:attribute>
            
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template match="tei:reg" />
    
    <xsl:template match="tei:p">
        <p>
            <xsl:apply-templates />
        </p>
        <xsl:call-template name="createNotesDiv"/>
    </xsl:template>
    
    <xsl:template match="tei:entryFree">
        <p class="entry">
            <xsl:attribute name="id">
                <!--<xsl:value-of select="@xml:id"/>-->
                <xsl:number format="000000" level="any"/>
            </xsl:attribute>
            <xsl:if test="tei:cit/tei:orth[@xml:lang='cs-x-transcr']">
                <xsl:attribute name="title">
                    <xsl:apply-templates select="tei:cit/tei:orth[@xml:lang='cs-x-transcr']" />
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates />
        </p>
        <xsl:call-template name="create-transcription-div" />
        <xsl:call-template name="createNotesDiv"/>
    </xsl:template>
    
    <xsl:template name="create-transcription-div">
        <xsl:if test=".//tei:reg">
            <div class="transcription"><xsl:apply-templates select=".//tei:reg" mode="transcription" /></div>
        </xsl:if>
    </xsl:template>
    
    <xsl:template match="tei:reg" mode="transcription">
        <span>
            <xsl:attribute name="class">
                <xsl:value-of select="@xml:lang"/>
                <xsl:if test="@rend">
                    <xsl:value-of select="concat(' ', @rend)"/>   
                </xsl:if>
            </xsl:attribute>
            
            <xsl:apply-templates />
        </span>
    </xsl:template>
    
    <xsl:template name="createNotesDiv">
        <xsl:if test="tei:note">
            <div class="notes"><xsl:apply-templates select="tei:note" mode="noteText" /></div>
        </xsl:if>
    </xsl:template>
    
    <xsl:template match="tei:hi">
        <xsl:element name="span">
            <xsl:attribute name="class">
                <xsl:choose>
                    <xsl:when test="@rend">
                        <xsl:value-of select="@rend"/>
                    </xsl:when>
                    <xsl:when test="@type">
                        <xsl:value-of select="@type"/>
                    </xsl:when>
                </xsl:choose>
            </xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="tei:pb">
        <span class="page"><xsl:value-of select="concat('[', @n, ']')"/></span>
        <xsl:if test="@rend='space'">
            <xsl:text> </xsl:text>
        </xsl:if>
    </xsl:template>
    

    
    <xsl:template match="tei:supplied">
        <xsl:element name="span">
            <xsl:attribute name="class"><xsl:text>supplied</xsl:text></xsl:attribute>
            <xsl:apply-templates />
        </xsl:element>
    </xsl:template>
    
    <xsl:template match="tei:ref">
        <a href="{@target}"><xsl:apply-templates /></a>
    </xsl:template>
    
</xsl:stylesheet>