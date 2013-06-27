<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:tei="http://www.tei-c.org/ns/1.0" xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0" xmlns:exist="http://exist.sourceforge.net/NS/exist" exclude-result-prefixes="xd tei nlp exist" version="1.0">
    <xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Jun 24, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="html"/>

    <xsl:strip-space elements="*"/>

    <!--<xsl:template match="/">
        <xsl:apply-templates select="//context[not(tei:entryFree)]" />
    </xsl:template>-->

    <xsl:template match="context[not(tei:entryFree)]">
        <div>
            <xsl:apply-templates/>
            <!--<hr />-->
        </div>
    </xsl:template>


    <xsl:param name="simple" select="false()"/>
    <xsl:variable name="zacatekRelace" select="'‹'"/>
    <xsl:variable name="konecRelace" select="'›'"/>


    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro ediční komentář ke konkrétní elektronické edici.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:div[@type='editorial' and @subtype='comment']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>editorial</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro incipit a explicit.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:div[@type='incipit' or @type='explicit']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:value-of select="@type"/>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblickou knihu.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='bible' and @subtype='book']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>book</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblickou kapitolu</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:div[@type='bible' and @subtype='chapter']">
        <xsl:element name="div">
            <xsl:attribute name="class">
                <xsl:text>chapter</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro biblický verš. Vygeneruje označení biblického verše (kapitola a verš).</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:seg[@type = 'bible' and @subtype='verse']">
        <xsl:element name="span">
            <xsl:attribute name="class">
                <xsl:text>verse</xsl:text>
            </xsl:attribute>
            <!--			<xsl:text>[</xsl:text>-->
            <xsl:value-of select="translate(substring-after(@xml:id, '.'), '.', ',')"/>
            <!--			<xsl:text>]</xsl:text>-->
        </xsl:element>
        <xsl:apply-templates/>
    </xsl:template>

    <xsl:template match="tei:anchor[@subtype='chapterStart']">
        <!--<xsl:element name="span">
			<xsl:attribute name="class"><xsl:text>verse</xsl:text></xsl:attribute>
			<xsl:value-of select="@n" />
<!-\-			<xsl:call-template name="substring-after-last">
				<xsl:with-param name="char" select="'.'" />
				<xsl:with-param name="string" select="@xml:id" />
			</xsl:call-template>
-\->
			<!-\-		<xsl:value-of select="translate(substring-after(@xml:id, '.'), '.', ',')"/>-\->
			</xsl:element>-->
    </xsl:template>

    <xsl:template match="tei:anchor[@type='bible' and @subtype='verse']">
        <xsl:element name="span">
            <xsl:attribute name="class"><xsl:text>verse</xsl:text></xsl:attribute>
            <xsl:variable name="ciloKapitoly">
                <xsl:choose>
                    <xsl:when test="preceding-sibling::anchor[@subtype='chapterStart']">
                        <xsl:value-of select="preceding-sibling::anchor[@subtype='chapterStart']/@n"/>
                    </xsl:when>
                    <xsl:when test="preceding-sibling::anchor[@subtype='chapter']">
                        <xsl:value-of select="preceding-sibling::anchor[@subtype='chapter']/@n"/>
                    </xsl:when>
                </xsl:choose>
            </xsl:variable>
            <xsl:value-of select="$ciloKapitoly"/>,<xsl:value-of select="@n"/>
            <!--			<xsl:call-template name="substring-after-last">
				<xsl:with-param name="char" select="'.'" />
				<xsl:with-param name="string" select="@xml:id" />
			</xsl:call-template>
-->
            <!--<xsl:value-of select="translate(substring-after(@xml:id, '.'), '.', ',')"/>-->
        </xsl:element>
    </xsl:template>


    <xsl:template name="substring-after-last">
        <xsl:param name="string"/>
        <xsl:param name="char"/>

        <xsl:choose>
            <xsl:when test="contains($string, $char)">
                <xsl:call-template name="substring-after-last">
                    <xsl:with-param name="string" select="substring-after($string, '.')"/>
                    <xsl:with-param name="char" select="$char"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$string"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro oddíl původního textu</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:div">
        <xsl:element name="div">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro seznam typu rejstřík.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:list[@type='index']">
        <xsl:element name="ul">
            <xsl:attribute name="class">
                <xsl:text>index</xsl:text>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro původní titul díla, uvedený v prameni.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:title">
        <xsl:element name="h2">
            <xsl:attribute name="style">
                <xsl:text>title</xsl:text>
            </xsl:attribute>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro nadpis.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:head">
        <xsl:element name="h3">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro ediční poznámku (obsahující pouze text). V hlavním textu se vytvoří pouze odkaz s číslem poznámky.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:note[text() or tei:hi]">
        <xsl:if test="not($simple)">
            <!-- pokud se poznámka vztahovala k delšími předchozímu textu, nikoli jenom bezprostřednímu slovu -->
            <xsl:if test="@from">
                <span class="info" title="konec pasáže s variantním překladem">
                    <xsl:value-of select="$konecRelace"/>
                </span>
            </xsl:if>
            <a href="#notesTool" class="collapseNotesTool">
                <span class="note">[<xsl:number level="any" count="hit//tei:note[text() or tei:hi]" format="1"/>]</span>
            </a>
        </xsl:if>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro emendační poznámku. V hlavním textu se vytvoří pouze odkaz s malým písmenem abecedy.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:note[tei:choice]">
        <xsl:if test="not($simple)">
            <!-- pokud se poznámka vztahovala k delšími předchozímu textu, nikoli jenom bezprostřednímu slovu -->
            <xsl:if test="@from">
                <span class="info" title="konec pasáže s variantním překladem">
                    <xsl:value-of select="$konecRelace"/>
                </span>
            </xsl:if>
            <a href="#notesTool" class="collapseNotesTool">
                <span class="note">[<xsl:number level="any" count="hit//tei:note[tei:choice]" format="a"/>]</span>
            </a>
        </xsl:if>
    </xsl:template>

   

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro položku v seznamu (rejstříku).</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:list/tei:item">
        <xsl:element name="li">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro odstavec.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:p">
        <p>
            <xsl:apply-templates/>
        </p>
    </xsl:template>


    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro verše, tj. řádky veršovaného textu.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:l">
        <xsl:element name="p">
            <xsl:attribute name="class">
                <xsl:text>line</xsl:text>
            </xsl:attribute>
            <xsl:choose>
                <xsl:when test="@n mod 5 = 0">
                    <xsl:element name="span">
                        <xsl:attribute name="class">
                            <xsl:text>verse</xsl:text>
                        </xsl:attribute>
                        <xsl:text>[</xsl:text>
                        <xsl:value-of select="@n"/>
                        <xsl:text>]&#160;</xsl:text>
                    </xsl:element>
                </xsl:when>
            </xsl:choose>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro text doplněný editorem.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:supplied">
        <span class="info supplied" title="text doplněný editorem">
            <xsl:text>[</xsl:text>
            <xsl:apply-templates/>
            <xsl:text>]</xsl:text>
        </span>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro zvýraznění textu.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:hi">
        <xsl:element name="span">
            <xsl:attribute name="class">
                <xsl:value-of select="@rend"/>
            </xsl:attribute>
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro kapitálku (obvykle první písmeno v knize, kapitole nebo odstavci).</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:c">
        <span class="initial">
            <xsl:apply-templates/>
        </span>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro přípisek.</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:add[@type='orientating']">
        <span class="info" title="textový orientátor">{<xsl:apply-templates/>}</span>
    </xsl:template>

    <xsl:template match="tei:add[@hand='#XY']">
        <span class="info" title="přípisek soudobého korektora">{<xsl:apply-templates/>}</span>
    </xsl:template>


    <xsl:template match="tei:add">
        <span class="info">
            <xsl:attribute name="title">
                <xsl:choose>
                    <xsl:when test="@place = 'inline'">meziřádkový</xsl:when>
                    <xsl:when test="@place = 'margin'">marginální</xsl:when>
                </xsl:choose>
                <xsl:text> </xsl:text>
                <xsl:text>přípisek</xsl:text>
                <xsl:text> </xsl:text>
                <xsl:choose>
                    <xsl:when test="@type = 'contemporaneous'">soudobou</xsl:when>
                    <xsl:when test="@type = 'non-contemporaneous'">mladší</xsl:when>
                </xsl:choose>
                <xsl:text> </xsl:text>
                <xsl:text>rukou</xsl:text>
            </xsl:attribute> {<xsl:apply-templates/>} </span>
    </xsl:template>

    <xsl:template match="tei:app[@from]">
        <span class="info" href="#" title="konec pasáže s variantním překladem">
            <xsl:value-of select="$konecRelace"/>
        </span>
        <xsl:apply-templates/>
    </xsl:template>


    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro text dalšího překladu v glosované bibli Klementinské.</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:rdg[@hand]">
        <!-- prvek app je pevně přifařen k předchozímu textu - je třeba vložit mezeru -->
        <xsl:text> </xsl:text>
        <span class="info nextTranslation variantniPreklad" title="variantní překlad">
            <xsl:apply-templates/>
        </span>
        <xsl:text> </xsl:text>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro začátek úseku (k němuž se bude vztahovat druhá překlad v BiblKlem).</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:anchor[@type='start']">
        <span class="info" title="začátek pasáže s variantním překladem">
            <xsl:value-of select="$zacatekRelace"/>
        </span>
    </xsl:template>


    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro text v cizím jazyce (ne česky).</xd:p>
        </xd:desc>
    </xd:doc>

    <xsl:template match="tei:foreign">
        <span class="info foreign" title="cizojazyčný text">
            <xsl:apply-templates/>
        </span>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro vynechanou část textu</xd:p>
        </xd:desc>
    </xd:doc>
    <!-- TODO: spojit značku pro vynechání textu s předchozím nebo následuícím textem a toto vše označit ako torzovité slovo -->
    <xsl:template match="tei:gap">
        <span class="info fragment" title="torzovité slovo">…</span>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro tabulku</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:table">
        <xsl:element name="table">
            <xsl:element name="tbody">
                <xsl:apply-templates/>
            </xsl:element>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro řádek v tabulce</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:row">
        <xsl:element name="tr">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>

    <xd:doc>
        <xd:desc>
            <xd:p>Šablona pro buňku v tabulce</xd:p>
        </xd:desc>
    </xd:doc>
    <xsl:template match="tei:cell">
        <xsl:element name="td">
            <xsl:apply-templates/>
        </xsl:element>
    </xsl:template>


</xsl:stylesheet>
