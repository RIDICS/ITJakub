using System.ComponentModel;

namespace Daliboris.Pomucky.Xml
{
	public enum SpaceHandling
	{
		/// <summary>
		/// <para>pokud nastane kombinace</para>
		/// <para>"&lt;foreign&gt;text &lt;/foreign&gt;&lt;pokračuje=></para>
		/// <para>"&lt;foreign&gt;text&lt;/foreign&gt;&lt; pokračuje</para>
		/// <para>(tj. text se přesunul za koncovou značku)</para>
		/// </summary>
		[Description("mezera se přesouvá vně značky")]
		SpaceIsMovinOutsideTag,
		/// <summary>
		/// <para>pokud nastane kombinace</para>
		/// <para>"text &lt;supplied&gt;... &lt;/supplied&gt;&lt; =></para>
		/// <para>text &lt;supplied&gt;... &lt;/supplied&gt;&lt;</para>
		/// <para>(tj. nic se nemění)</para>
		/// </summary>
		[Description("před značkou se mezera nepřesouvá")]
		SpaceIsNotMovingBeforeThisTag,

		/// <summary>
		///<para>pokud nastane kombinace</para>
		///<para>"text &lt;supplied&gt;... &lt;/supplied&gt;&lt;note&gt;poznámka&lt;/note&gt;text" =></para>
		///<para>"text &lt;supplied&gt;...&lt;/supplied&gt;&lt;note&gt;poznámka&lt;/note&gt; text"</para>
		/// <para>(tj. mezera se přesouvá ze značky supplied za note)</para>
		/// </summary>
		[Description("mezera se přesouvá za značku")]
		SpaceIsMovingAfterThisTag
	}
}