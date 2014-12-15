<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:output method="html" indent="yes" encoding="UTF-8" />
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:template match="/">
		<xsl:element name="html">
			<xsl:element name="head">
				<link href="Site.css" rel="stylesheet" type="text/css" ></link>
				<script type="text/javascript" src="http://code.jquery.com/jquery-latest.js"></script>
			</xsl:element>
			<xsl:element name="body">
				<xsl:element name="div">
					<xsl:attribute name="id"><xsl:text>wrapper</xsl:text></xsl:attribute>
				<xsl:element name="div">
					<xsl:attribute name="id"><xsl:text>Content</xsl:text></xsl:attribute>
					<xsl:element name="div">
						<xsl:attribute name="id"><xsl:text>mainContent</xsl:text></xsl:attribute>
						<xsl:apply-templates select="TEI/text/body" />
						<xsl:apply-templates select="TEI/text/body" mode="poznamky" />
					</xsl:element>
				</xsl:element>
				</xsl:element>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="body">
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="div[@type='editorial' and @subtype='comment']">
		<xsl:element name="div">
			<xsl:attribute name="style"><xsl:text>editorial</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="div[@type='bible' and @subtype='book']">
		<xsl:element name="div">
			<xsl:attribute name="style"><xsl:text>book</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="div[@type='bible' and @subtype='chapter']">
		<xsl:element name="div">
			<xsl:attribute name="style"><xsl:text>chapter</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="div">
		<xsl:element name="div">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="list[@type='index']">
		<xsl:element name="ul">
			<xsl:attribute name="style"><xsl:text>index</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="head">
		<xsl:element name="h3">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xsl:template match="note[text()]">
		<a href="#" class="collapseNotesTool">
			<span class="note">[<xsl:number level="any"  count="note[text()]" format="1"/>]</span>
			</a>
	</xsl:template>

	<xsl:template match="note[choice]">
		<a href="#" class="collapseNotesTool">
			<span class="note">[<xsl:number level="any"  count="note[choice]" format="a"/>]</span>
		</a>
	</xsl:template>



	<xd:doc scope="component">
		<xd:desc><xd:p>Dočasné řešení</xd:p></xd:desc>
	</xd:doc>
	
	<xsl:template match="pb">
		<xsl:element name="span">
			<xsl:attribute name="style"><xsl:text>pb</xsl:text></xsl:attribute>
		<xsl:text>|</xsl:text>
				<xsl:value-of select="@n"/>
		<xsl:text>|</xsl:text>
		</xsl:element>
	</xsl:template>

	<xsl:template match="list/item">
		<xsl:element name="li">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="p">
		<xsl:copy>
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	

	<xsl:template match="hi">
		<xsl:element name="span">
			<xsl:attribute name="class"><xsl:value-of select="@rend"/></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="c">
		<span class="initial">
			<xsl:apply-templates />
			</span>
	</xsl:template>


<xd:doc scope="component">
	<xd:desc><xd:p>Šablony pro generování oddílu s poznámkovým aparátem. Šablony jsou označeny modem <xd:b>poznamka</xd:b>.</xd:p> </xd:desc>
</xd:doc>
	
	<xsl:template match="body" mode="poznamky">
		<div id="notesTool">
			<table width="100%">
				<colgroup>
					<col width="10%" />
					<col width="90%" />
				</colgroup>
				<tr>
					<td colspan="2" style="text-align: right;">
						<a href="#" class="closeNotesTool">
							<img src="/Devedu.Ujc.Web/Images/close_ico.jpg" alt="X" /></a>
					</td>
				</tr>
				<xsl:apply-templates mode="poznamky" select="//note[choice]" />
				<xsl:apply-templates mode="poznamky" select="//note[text()]" />
			</table>
		</div>
		
		<script type="text/javascript"> <xsl:text>
        $("div#notesTool").hide();
        $("a.collapseNotesTool").click(function () {
            if ($("div#notesTool").is(":hidden") == true) {
                $("div#notesTool").toggle("slow");
            }
        });
        $("a.closeNotesTool").click(function () {
            $("div#notesTool").toggle("slow");
        });

        $("#btnGo").click(function () {
            var input = $("#txtPage").val();
            window.location.href = '' + '/' + input
        });
</xsl:text>
		</script>
		</xsl:template>

	<xsl:template match="note[text()]" mode="poznamky">
		<tr>
			<td><xsl:number level="any" count="note[text()]" format="1"/></td>
			<td>
				<xsl:apply-templates select="text()" />
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="note[choice]" mode="poznamky">
		<tr>
			<td><xsl:number level="any" count="note[choice]" format="a"/></td>
			<td>
				<span class="italic"><xsl:apply-templates select="choice/corr" /></span>] <xsl:apply-templates select="choice/sic"/>
			</td>
		</tr>
	</xsl:template>
	

</xsl:stylesheet>