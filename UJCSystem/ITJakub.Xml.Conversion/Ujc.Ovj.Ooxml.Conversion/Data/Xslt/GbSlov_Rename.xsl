<?xml version="1.0" encoding="utf-8"?>
<axsl:stylesheet xmlns:axsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
   <axsl:output indent="yes" method="xml" standalone="yes"/>
   <axsl:strip-space elements="*"/>
   <axsl:preserve-space elements="text location refsource" />
   <axsl:template match="/">
      <axsl:apply-templates/>
   </axsl:template>
   <xsl:template match="body">
      <dictionary>
         <axsl:attribute name="name">
            <axsl:text>GbSlov</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </dictionary>
   </xsl:template>
		
   <axsl:template match="Zahlavi_strany">
      <axsl:element name="pb">
         <axsl:attribute name="n">
            <axsl:value-of select="cislo_strany"/>
         </axsl:attribute>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="cislo_strany">
      <axsl:element name="pb">
         <axsl:attribute name="n">
            <axsl:value-of select="text()"/>
         </axsl:attribute>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="cislo_homonyma">
      <axsl:element name="hom">
         <axsl:attribute name="id">
            <axsl:value-of select="text()" />
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="heslove_slovo_zkracene_delimitator">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
	
   <axsl:template match="Heslova_stat">
      <axsl:element name="entry">
         <axsl:attribute name="type">
            <axsl:text>full</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="Odkazova_stat">
      <axsl:element name="entry">
         <axsl:attribute name="type">
            <axsl:text>ref</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="heslove_slovo_delimitator">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="heslove_slovo_rozepsane_delimitator">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="tucne">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="delimitator">
      <axsl:element name="delim">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="delimitator_kurziva">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="vyznam">
      <axsl:element name="def">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

   <axsl:template match="slovni_druh">
      <axsl:element name="pos">
         <axsl:attribute name="rend">
            <axsl:text>norm</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="podhesli">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="type">
            <axsl:text>subentry</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="podhesli_rozepsane">
      <axsl:element name="hw">
         <axsl:attribute name="type">
            <axsl:text>subentry</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="podhesli_zkracene">
      <axsl:element name="hw">
         <axsl:attribute name="type">
            <axsl:text>subentry</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="podhesli_delimitator">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="slovni_druh_kurziva">
      <axsl:element name="pos">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

   <axsl:template match="Heslove_zahlavi">
      <axsl:element name="entryhead">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Koncovy_odkaz">
      <axsl:element name="appendix">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Odkazove_zahlavi">
      <axsl:element name="entryhead">
         <axsl:attribute name="type">
            <axsl:text>ref</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="Pismeno">
      <axsl:element name="milestone">
         <axsl:attribute name="type">
            <axsl:text>letter</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Poznamka">
      <axsl:element name="note">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Specifikace_pismeno">
      <axsl:element name="senseGrp">
         <axsl:attribute name="type">
            <axsl:text>letter</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="Specifikace_rimska_cislice">
      <axsl:element name="senseGrp">
         <axsl:attribute name="type">
            <axsl:text>number</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <xsl:template match="Starocesky_slovnik"/>
		
   <axsl:template match="Vyznamovy_odstavec">
      <axsl:element name="sense">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
		
   <xsl:template match="Obrazek"/>
		
   <xsl:template match="Popisek_obrazku"/>
		
		
		
   <axsl:template match="biblicke_misto">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>bible</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="biblicke_misto_kurziva">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>bible</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="delimitator_netucne">
      <axsl:element name="delim">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="delimitator_tucne">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="delimitator_tucne_zkracene">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="rozepsane_delimitator_tucne">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
			
   <axsl:template match="delimitator_vyznamu">
      <axsl:element name="delim">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="delimitator_gill">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="delimitator_rozepsane_gill">
      <axsl:element name="delim">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="forma">
      <axsl:element name="form">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="forma_rozepsane">
      <axsl:element name="form">
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="forma_zkracene">
      <axsl:element name="form">
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="forma_gill">
      <axsl:element name="form">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="frazem">
      <axsl:element name="hwcolloc">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="frazem_rozepsane">
      <axsl:element name="hwcolloc">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="frazem_zkracene">
      <axsl:element name="hwcolloc">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="gill">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_kurziva">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_kurziva_rozepsane">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_kurziva_zkracene">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_netucne">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>norm</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_netucne_rozepsane">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>norm</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_netucne_zkracene">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>norm</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_rozepsane">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="heslove_slovo_zkracene">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="horni_index">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>sup</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="kapitalky">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>pram</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>cap</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="kurziva">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="latinsky_doklad">
      <axsl:element name="ex">
         <axsl:attribute name="lang">
            <axsl:text>lat</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="latinsky_ekvivalent">
      <axsl:element name="equi">
         <axsl:attribute name="lang">
            <axsl:text>lat</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="lokace">
      <axsl:element name="location">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="morfologicka_charakteristika">
      <axsl:element name="morph">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="charakteristika">
      <axsl:element name="gloss">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="charakteristika_kurziva">
      <axsl:element name="gloss">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="nemecky_doklad">
      <axsl:element name="ex">
         <axsl:attribute name="lang">
            <axsl:text>ger</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="nemecky_ekvivalent">
      <axsl:element name="equi">
         <axsl:attribute name="lang">
            <axsl:text>ger</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="nonparej">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>nonp</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="novocesky_preklad">
      <axsl:element name="translation">
         <axsl:attribute name="lang">
            <axsl:text>cze</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="odborna_literatura">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>prof</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="odborna_literatura_kurziva">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>prof</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="odborna_literatura_novocesky_preklad">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>prof</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="odborna_literatura_vyznam">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>prof</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="odkaz">
      <axsl:element name="xref">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="odkaz_kurziva">
      <axsl:element name="xref">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
   
   <axsl:template match="odkaz_tucne">
      <axsl:element name="xref">
         <axsl:attribute name="rend">
            <axsl:text>bo</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
		
   <axsl:template match="odkaz_mimo_StcS">
      <axsl:element name="xref">
         <axsl:attribute name="target">
            <axsl:text>external</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="paradigmaticka_odvozenina">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="type">
            <axsl:text>subentry</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="paradigmaticka_odvozenina_rozepsane">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="type">
            <axsl:text>subentry</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="paradigmaticka_odvozenina_zkracene">
      <axsl:element name="hw">
         <axsl:attribute name="rend">
            <axsl:text>snserif</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="type">
            <axsl:text>subentry</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="pramen">
      <axsl:element name="refsource">
         <!--<axsl:attribute name="type">
            <axsl:text>pram</axsl:text>
         </axsl:attribute>-->
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="pramen_kurziva">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>pram</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="pramen_zkraceny">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>pram</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="rok">
      <axsl:element name="year">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="signatura">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>signature</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="signatura_kurziva">
      <axsl:element name="location">
         <axsl:attribute name="type">
            <axsl:text>signature</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="slovni_druh_nonparej">
      <axsl:element name="pos">
         <axsl:attribute name="rend">
            <axsl:text>nonp</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="specifikace">
      <axsl:element name="spec">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="*">
      <axsl:copy>
         <axsl:copy-of select="@*"/>
         <axsl:apply-templates/>
      </axsl:copy>
   </axsl:template>
		
   <axsl:template match="text">
      <axsl:element name="text">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <xsl:template match="Pismeno/text">
      <axsl:apply-templates/>
   </xsl:template>
		
   <axsl:template match="vyskyt_hesla">
      <axsl:element name="xref">
         <axsl:attribute name="type">
            <axsl:text>hw</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
				
   <axsl:template match="vyznamove_zahlavi">
      <axsl:element name="val">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="zkratka">
      <axsl:element name="abbr">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="zkratka_kurziva">
      <axsl:element name="abbr">
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="zkratka_nonparej">
      <axsl:element name="abbr">
         <axsl:attribute name="rend">
            <axsl:text>nonp</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="spojitelnost">
      <axsl:element name="val">
         <axsl:attribute name="type">
            <axsl:text>semantic</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="valence">
      <axsl:element name="val">
         <axsl:attribute name="type">
            <axsl:text>formal</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="prolozene">
      <axsl:element name="text">
         <axsl:attribute name="rend">
            <axsl:text>spaced</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
		
   <axsl:template match="pamatka">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>pam</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="pramen_SSL">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>SSL</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
   <axsl:template match="pramen_SLL">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>SSL</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
		
   <axsl:template match="delimitator_rozepsane_tucne">
      <axsl:element name="refsource">
         <axsl:attribute name="rend">
            <axsl:text>bold</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="delimitator_zkracene_netucne">
      <axsl:element name="refsource">
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="delimitator_zkracene_tucne">
      <axsl:element name="refsource">
         <axsl:attribute name="form">
            <axsl:text>short</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
			
   <axsl:template match="periodikum">
      <axsl:element name="refsource">
         <axsl:attribute name="rend">
            <axsl:text>bold</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="type">
            <axsl:text>per</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
   <axsl:template match="periodikum_kurziva">
      <axsl:element name="refsource">
         <axsl:attribute name="type">
            <axsl:text>per</axsl:text>
         </axsl:attribute>
         <axsl:attribute name="rend">
            <axsl:text>it</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>

		
   <axsl:template match="delimitator_rozepsane_netucne">
      <axsl:element name="delim">
         <axsl:attribute name="form">
            <axsl:text>restored</axsl:text>
         </axsl:attribute>
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <axsl:template match="delimitator_zkracene">
      <axsl:element name="delim">
         <axsl:apply-templates/>
      </axsl:element>
   </axsl:template>
		
		
   <xsl:template match="annotation_reference"/>
	
</axsl:stylesheet>