<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:dcr="http://www.isocat.org/ns/dcr" 
    xmlns="http://www.tei-c.org/ns/1.0"
    xmlns:xml="http://www.w3.org/XML/1998/namespace"
    exclude-result-prefixes="xs"
    version="1.0">
    
    <xsl:include href="TEI_Common.xsl"/>
    
    <xsl:output indent="yes"/>
<!--    <xsl:strip-space elements="TEI additional address adminInfo altGrp altIdentifier analytic app appInfo application arc argument attDef attList availability back biblFull biblStruct bicond binding bindingDesc body broadcast cRefPattern calendar calendarDesc castGroup castList category certainty char charDecl charProp choice cit classDecl classSpec classes climate cond constraintSpec correction custodialHist decoDesc dimensions div div1 div2 div3 div4 div5 div6 div7 divGen docTitle eLeaf eTree editionStmt editorialDecl elementSpec encodingDesc entry epigraph epilogue equipment event exemplum fDecl fLib facsimile figure fileDesc floatingText forest front fs fsConstraints fsDecl fsdDecl fvLib gap glyph graph graphic group handDesc handNotes history hom hyphenation iNode if imprint incident index interpGrp interpretation join joinGrp keywords kinesic langKnowledge langUsage layoutDesc leaf lg linkGrp list listApp listBibl listChange listEvent listForest listNym listOrg listPerson listPlace listPrefixDef listRef listRelation listTranspose listWit location locusGrp macroSpec media metDecl moduleRef moduleSpec monogr msContents msDesc msIdentifier msItem msItemStruct msPart namespace node normalization notatedMusic notesStmt nym objectDesc org particDesc performance person personGrp physDesc place population postscript precision prefixDef profileDesc projectDesc prologue publicationStmt quotation rdgGrp recordHist recording recordingStmt refsDecl relatedItem relation relationGrp remarks respStmt respons revisionDesc root row samplingDecl schemaSpec scriptDesc scriptStmt seal sealDesc segmentation seriesStmt set setting settingDesc sourceDesc sourceDoc sp spGrp space spanGrp specGrp specList state stdVals styleDefDecl subst substJoin superEntry supportDesc surface surfaceGrp table tagsDecl taxonomy teiCorpus teiHeader terrain text textClass textDesc timeline titlePage titleStmt trait transpose tree triangle typeDesc vAlt vColl vDefault vLabel vMerge vNot vRange valItem valList vocal sense "/>-->
    <xsl:strip-space elements="*"/>
    
    <xsl:template match="/">
        <TEI n="{{A60FF9E4-36F0-4770-89AB-E710121B8D4E}}">
            <teiHeader>
                <fileDesc n="{{A60FF9E4-36F0-4770-89AB-E710121B8D4E}}">
                    <titleStmt>
                        <title>Staročeský slovník</title>
                    </titleStmt>
                    <publicationStmt>
                        <publisher>Ústav pro jazyk český AV ČR, v. v. i.</publisher>
                        <pubPlace>Praha</pubPlace>
                        <date>1968–2006</date>
                        <availability status="restricted">
                            <p>Tato elektronická edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském, a je určena pouze k nekomerčním účelům.</p> 
                        </availability>
                    </publicationStmt>
                    <sourceDesc>
                        <listBibl>
                            <bibl type="acronym" subtype="source">StčS</bibl> 
                        </listBibl>
                        <bibl>Staročeský slovník. Hesla s náslovním <hi rend="italic">N</hi>–<hi rend="italic">při-</hi> a <hi rend="italic">A</hi>–<hi rend="italic">G</hi>. 1968–2006.</bibl>
                    </sourceDesc>
                </fileDesc>
                <xsl:call-template name="InsertEndocingDesc"/>
                <profileDesc>
                    <textClass>
                    	<catRef target="#taxonomy-dictionary-contemporary #output-dictionary"/>
                    </textClass>
                </profileDesc>
            </teiHeader>
            <text>
                <body>
                    <xsl:apply-templates />
                </body>
            </text>
        </TEI>
    </xsl:template>
    
    <xsl:template match="div1[@type='letter']">
        <div xml:id="{@id}" type='letter'>
            <head><xsl:value-of select="@text"/></head>
            <xsl:apply-templates />
        </div>
    </xsl:template>
    
    <xsl:template match="div1[@type='letter']/text[1]" />
    
    
    <xsl:template match="entry">
        <entryFree xml:id="{@id}">
            <xsl:if test="@use">
                <xsl:attribute name="type">
                    <xsl:value-of select="@use"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates />
        </entryFree>
    </xsl:template>
    
    <xsl:template match="hw">
        <form xml:id="{@id}">
        	<xsl:if test="@rend">
        	<xsl:attribute name="rend">
        			<xsl:value-of select="@rend"/>
        	</xsl:attribute>
        	</xsl:if>
        	<xsl:if test="@type">
        		<xsl:attribute name="type">
        			<xsl:value-of select="@type"/>
        		</xsl:attribute>
        	</xsl:if>
        	<orth><xsl:apply-templates /></orth>
        </form>
    </xsl:template>
    
    <xsl:template match="pos">
        <gramGrp>
            <xsl:call-template name="GeneratePos" />
        </gramGrp>
    </xsl:template>
    
    <xsl:template match="senseGrp">
        <sense>
            <xsl:attribute name="level">
                <xsl:choose>
                    <xsl:when test="parent::entry">
                        <xsl:text>1</xsl:text>
                    </xsl:when>
                    <xsl:when test="parent::senseGrp">
                        <xsl:text>2</xsl:text>
                    </xsl:when>
                </xsl:choose>
            </xsl:attribute>
            <xsl:if test="child::delim[1]">
                <num>
                    <xsl:apply-templates select="child::delim[1]" mode="obsah" />
                </num>
            </xsl:if>
            <def>
                <xsl:apply-templates select="child::text[1]" mode="obsah" />
            </def>
            <xsl:apply-templates />
        </sense>
    </xsl:template>
    
    <xsl:template match="senseGrp/delim[1]" mode="obsah">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="senseGrp/delim[1]" />
    
    <xsl:template match="senseGrp/text[1]" mode="obsah">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="senseGrp/text[1]" />

    <xsl:template match="sense">
        <sense>
            <xsl:attribute name="level">
                <xsl:choose>
                    <xsl:when test="parent::senses/parent::entry">
                        <xsl:text>1</xsl:text>
                    </xsl:when>
                    <xsl:when test="parent::senses/parent::senseGrp/parent::senseGrp">
                        <xsl:text>3</xsl:text>
                    </xsl:when>
                    <xsl:when test="parent::senses/parent::senseGrp">
                        <xsl:text>2</xsl:text>
                    </xsl:when>
                </xsl:choose>
            </xsl:attribute>
            <xsl:apply-templates />
        </sense>
    </xsl:template>
    
    <xsl:template match="sense/delim[1]">
        <num><xsl:apply-templates /></num>
    </xsl:template>
    
    <xsl:template match="def">
        <def rend="it">
            <xsl:apply-templates />
        </def>
    </xsl:template>
    
    <xsl:template match="def[@lang='oldcze']">
        <def rend="it">
            <cit>
                <xr type="cf">
                    <ref><xsl:apply-templates /></ref>
                </xr>
            </cit>
        </def>
    </xsl:template>

    <xsl:template name="GeneratePos">
        <xsl:variable name="text">
            <xsl:value-of select="normalize-space(.)"/>
        </xsl:variable>

        <pos>
        <xsl:choose>

            <xsl:when test="$text = 'adj.'">
                <xsl:attribute name="norm">
                    <xsl:text>adjective</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1230</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$text = 'adv.'">
                <xsl:attribute name="norm">
                    <xsl:text>adverb</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1232</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$text = 'n.'">
                <xsl:attribute name="norm">
                    <xsl:text>noun</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1333</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$text = 'm.'">
                <xsl:attribute name="norm">
                    <xsl:text>noun</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1333</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$text = 'f.'">
                <xsl:attribute name="norm">
                    <xsl:text>noun</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1333</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$text = 'interj.'">
                <xsl:attribute name="norm">
                    <xsl:text>interjection</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1318</xsl:text>
                </xsl:attribute>
            </xsl:when>
            
            <xsl:when test="$text = 'ipf.'">
                <xsl:attribute name="norm">
                    <xsl:text>verb</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1424</xsl:text>
                </xsl:attribute>
            </xsl:when>
            
            <xsl:when test="$text = 'pf.'">
                <xsl:attribute name="norm">
                    <xsl:text>verb</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1424</xsl:text>
                </xsl:attribute>
            </xsl:when>
            
            <xsl:when test="$text = 'pron.'">
                <xsl:attribute name="norm">
                    <xsl:text>pronoun</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1370</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <!-- 
            <xsl:when test="$text = 'indecl.'">
                <xsl:attribute name="norm">
                    <xsl:text>noun</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1333</xsl:text>
                </xsl:attribute>
            </xsl:when>
             -->
            
            <xsl:when test="$text = 'konj.'">
                <xsl:attribute name="norm">
                    <xsl:text>conjunction</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1260</xsl:text>
                </xsl:attribute>
            </xsl:when>
            <xsl:when test="$text = 'partik.'">
                <xsl:attribute name="norm">
                    <xsl:text>particle</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="datcat" namespace="http://www.isocat.org/ns/dcr">
                    <xsl:text>http://www.isocat.org/datcat/DC-1342</xsl:text>
                </xsl:attribute>
            </xsl:when>
            
        </xsl:choose>
        <abbr rend="nonp">
            <xsl:apply-templates />
        </abbr>
        </pos>
    </xsl:template>
    
    <xsl:template match="note">
        <note>
            <xsl:apply-templates />
        </note>
    </xsl:template>

    <xsl:template match="entryhead/text">
        <hi>
            <xsl:attribute name="rend">
                <xsl:choose>
                    <xsl:when test="not(@rend)">
                        <xsl:text>none</xsl:text>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="@rend"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <xsl:apply-templates />
        </hi>
    </xsl:template>

    <xsl:template match="resp">
        <note type="response"><persName full="abb"><xsl:apply-templates /></persName></note>
    </xsl:template>
    
    <xsl:template match="text[@rend]" priority="20">
        <hi rend="{@rend}">
            <xsl:if test="@type = 'hidden'">
                <xsl:attribute name="rend">
                    <xsl:value-of select="concat(@rend, ' ', @type)"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates />
        </hi>
    </xsl:template>
    
    <xsl:template match="xref">
        <xr type="cf"> <!-- StčS -->
            <xsl:attribute name="norm">
                <xsl:choose>
                    <xsl:when test="@hw">
                        <xsl:value-of select="@hw"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="."/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
           
            <ref><xsl:apply-templates /></ref>
        </xr>
    </xsl:template>
    
    <!-- DOčasné řešení; v TEI P5 nelze v poznámce použít odkaz na jiné slovníkové heslo -->
    <xsl:template match="note/xref">
        <cit type="cf"> <!-- StčS -->
            <xsl:attribute name="n"> <!-- u xr jde o atribut @norm  -->
                <xsl:choose>
                    <xsl:when test="@hw">
                        <xsl:value-of select="@hw"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="."/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            
            <ref><xsl:apply-templates /></ref>
        </cit>
    </xsl:template>
    
    <xsl:template match="comment[@type='hidden']">
        <note type="hidden">
            <xsl:apply-templates />
        </note>
    </xsl:template>
    
    <xsl:template match="refsource[@type='hidden']" priority="10">
        <xr type="source" rend="hidden">
            <xsl:attribute name="norm">
            <xsl:choose>
                <xsl:when test="@hw">
                    <xsl:value-of select="@hw"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="normalize-space(.)"/>
                </xsl:otherwise>
            </xsl:choose>
            </xsl:attribute>
            <ref><xsl:apply-templates /></ref>
        </xr>
    </xsl:template>

    <xsl:template match="note/refsource">
        <cit type="source">
            <xsl:if test="@type">
                <xsl:attribute name="type">
                    <xsl:value-of select="@type" />
                </xsl:attribute>
            </xsl:if>
            <xsl:attribute name="n">
                <xsl:choose>
                    <xsl:when test="@hw">
                        <xsl:value-of select="@hw"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="normalize-space(.)"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <ref><xsl:apply-templates /></ref>
        </cit>
    </xsl:template>
    <xsl:template match="refsource">
        <xr type="source">
            <xsl:if test="@type">
                <xsl:attribute name="type">
                    <xsl:value-of select="@type" />
                </xsl:attribute>
            </xsl:if>
            <xsl:if test="@rend">
                <xsl:attribute name="rend">
                    <xsl:value-of select="@rend"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:attribute name="norm">
                <xsl:choose>
                    <xsl:when test="@hw">
                        <xsl:value-of select="@hw"/>
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="normalize-space(.)"/>
                    </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <ref><xsl:apply-templates /></ref>
        </xr>
    </xsl:template>
    
    <xsl:template match="morph">
        <oVar type="morph">
            <xsl:apply-templates />
        </oVar>
    </xsl:template>
    
    <xsl:template match="entryhead/delim[@rend]">
        <hi rend="{@rend}"><xsl:apply-templates /></hi>
    </xsl:template>
    
    <xsl:template match="action">
        <note type="action">
            <xsl:apply-templates />
        </note>
    </xsl:template>
    
    <xsl:template match="hwcolloc">
        <form type="compound">
        	<xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
        	<xsl:value-of select="concat(ancestor::entry[1]/@id, '.hc')"/>
        	<xsl:number from="entry" level="any"  format="1"/>
        </xsl:attribute>
            <xsl:apply-templates />
        </form>
    </xsl:template>
    <xsl:template match="motivSect">
        <etym>
            <xsl:apply-templates />
        </etym>
    </xsl:template>
    
    <xsl:template match="val">
        <term type="valency">
            <xsl:if test="@type">
                <xsl:attribute name="subtype">
                    <xsl:value-of select="@type"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates />
        </term>
    </xsl:template>
    
</xsl:stylesheet>