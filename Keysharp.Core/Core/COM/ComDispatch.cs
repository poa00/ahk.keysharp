﻿using System;
using System.Runtime.InteropServices;
using ct = System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Keysharp.Core.COM
{
	/*
	    namespace ManagedWinapi
	    {
	    /// <summary>
	    /// This class contains methods to identify type name, functions and variables
	    /// of a wrapped COM object (that appears as System.__COMObject in the debugger).
	    /// </summary>
	    public class COMTypeInformation
	    {
	        IDispatch dispatch;
	        ct.ITypeInfo typeInfo;
	        private const int LCID_US_ENGLISH = 0x409;

	        /// <summary>
	        /// Create a new COMTypeInformation object for the given COM Object.
	        /// </summary>
	        public COMTypeInformation(object comObject)
	        {
	            dispatch = comObject as IDispatch;

	            if (dispatch == null) throw new Exception("Object is not a COM Object");

	            int typeInfoCount;
	            int hr = dispatch.GetTypeInfoCount(out typeInfoCount);

	            if (hr < 0) throw new COMException("GetTypeInfoCount failed", hr);

	            if (typeInfoCount != 1) throw new Exception("No TypeInfo present");

	            hr = dispatch.GetTypeInfo(0, LCID_US_ENGLISH, out typeInfo);

	            if (hr < 0) throw new COMException("GetTypeInfo failed", hr);
	        }

	        /// <summary>
	        /// The type name of the COM object.
	        /// </summary>
	        public string TypeName
	        {
	            get
	            {
	                string name, dummy1, dummy3;
	                int dummy2;
	                typeInfo.GetDocumentation(-1, out name, out dummy1, out dummy2, out dummy3);
	                return name;
	            }
	        }

	        /// <summary>
	        /// The names of the exported functions of this COM object.
	        /// </summary>
	        public IList<string> FunctionNames
	        {
	            get
	            {
	                List<string> result = new List<String>();

	                for (int jj = 0; ; jj++)
	                {
	                    IntPtr fncdesc;

	                    try
	                    {
	                        typeInfo.GetFuncDesc(jj, out fncdesc);
	                    }
	                    catch (COMException) { break; }

	                    ct.FUNCDESC fd = (ct.FUNCDESC)Marshal.PtrToStructure(fncdesc, typeof(ct.FUNCDESC));
	                    string[] tmp = new string[1];
	                    int cnt;
	                    typeInfo.GetNames(fd.memid, tmp, tmp.Length, out cnt);

	                    if (cnt == 1)
	                        result.Add(tmp[0]);

	                    typeInfo.ReleaseFuncDesc(fncdesc);
	                }

	                return result;
	            }
	        }

	        /// <summary>
	        /// The names of the exported variables of this COM object.
	        /// </summary>
	        public IList<string> VariableNames
	        {
	            get
	            {
	                List<string> result = new List<String>();

	                for (int jj = 0; ; jj++)
	                {
	                    IntPtr vardesc;

	                    try
	                    {
	                        typeInfo.GetVarDesc(jj, out vardesc);
	                    }
	                    catch (COMException) { break; }

	                    ct.VARDESC vd = (ct.VARDESC)Marshal.PtrToStructure(vardesc, typeof(ct.VARDESC));
	                    string[] tmp = new string[1];
	                    int cnt;
	                    typeInfo.GetNames(vd.memid, tmp, tmp.Length, out cnt);

	                    if (cnt == 1)
	                        result.Add(tmp[0]);

	                    typeInfo.ReleaseFuncDesc(vardesc);
	                }

	                return result;
	            }
	        }
	    }
	    }
	*/

	/// <summary>
	/// The IDispatch interface.
	/// This was taken loosely from https://github.com/PowerShell/PowerShell/blob/master/src/System.Management.Automation/engine/COM/
	/// under the MIT license.
	/// </summary>
	[Guid("00020400-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IDispatch
	{
		[PreserveSig]
		int GetTypeInfoCount(out int info);

		[PreserveSig]
		int GetTypeInfo(int iTInfo, int lcid, out ct.ITypeInfo? ppTInfo);

		[PreserveSig]
		int GetIDsOfNames([MarshalAs(UnmanagedType.LPStruct)] Guid riid,
						  [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] string[] names,
						  int cNames, int lcid,
						  [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] rgDispId);

		int Invoke(int dispIdMember,
				   [MarshalAs(UnmanagedType.LPStruct)]
				   Guid riid,
				   int lcid,
				   ct.INVOKEKIND wFlags,
				   ref ct.DISPPARAMS pDispParams,
				   IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}

	[ComImport]
	[Guid("B196B283-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IProvideClassInfo
	{
		[PreserveSig]
		int GetClassInfo(out ct.ITypeInfo typeInfo);
	}

	[Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IServiceProvider
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int QueryService(
			[In] ref Guid guidService,
			[In] ref Guid riid,
			[Out] out IntPtr ppvObject);
	}

	//[StructLayout(LayoutKind.Sequential)]
	//internal struct VARIANT
	//{
	//  public ushort vt;
	//  public ushort r0;
	//  public ushort r1;
	//  public ushort r2;
	//  public IntPtr ptr0;
	//  public IntPtr ptr1;
	//}

	/// <summary>
	/// Solution for event handling taken from the answer to my post at:
	/// https://stackoverflow.com/questions/77010721/how-to-late-bind-an-event-sink-for-a-com-object-of-unknown-type-at-runtime-in-c
	/// </summary>
	internal class Dispatcher : IDisposable, IDispatch, ICustomQueryInterface
	{
		private const int E_NOTIMPL = unchecked((int)0x80004001);
		private static readonly Guid IID_IManagedObject = new ("{C3FCC19E-A970-11D2-8B5A-00A0C9B7C9C4}");
		private ct.IConnectionPoint connection;
		private int cookie;
		private bool disposedValue;
		private Guid interfaceID;
		private ct.ITypeInfo typeInfo = null;

		public ComObject Co { get; }

		internal Guid InterfaceId => interfaceID;

		internal Dispatcher(ComObject cobj)
		{
			ArgumentNullException.ThrowIfNull(cobj);
			var container = cobj.Ptr;

			if (container is not ct.IConnectionPointContainer cpContainer)
				throw new ValueError($"The passed in COM object of type {container.GetType()} was not of type IConnectionPointContainer.");

			Co = cobj;
			ct.ITypeInfo ti;

			if (container is IProvideClassInfo ipci)
				_ = ipci.GetClassInfo(out ti);
			else if (container is IDispatch idisp)
				_ = idisp.GetTypeInfo(0, 0, out ti);
			else
				throw new ValueError($"The passed in COM object of type {container.GetType()} was not of type IProvideClassInfo or IDispatch");

			ti.GetTypeAttr(out var typeAttr);
			ct.TYPEATTR attr = (ct.TYPEATTR)Marshal.PtrToStructure(typeAttr, typeof(ct.TYPEATTR));
			var cImplTypes = attr.cImplTypes;
			ti.ReleaseTypeAttr(typeAttr);

			for (var j = 0; j < cImplTypes; j++)
			{
				try
				{
					ti.GetImplTypeFlags(j, out var typeFlags);

					if (typeFlags.HasFlag(ct.IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT) && typeFlags.HasFlag(ct.IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE))
					{
						ti.GetRefTypeOfImplType(j, out var href);
						ti.GetRefTypeInfo(href, out var ppTI);
						ppTI.GetTypeAttr(out typeAttr);
						attr = (ct.TYPEATTR)Marshal.PtrToStructure(typeAttr, typeof(ct.TYPEATTR));

						if (attr.typekind == ct.TYPEKIND.TKIND_DISPATCH)
						{
							cpContainer.FindConnectionPoint(ref attr.guid, out var con);

							if (con != null)
							{
								interfaceID = attr.guid;
								typeInfo = ppTI;
								con.Advise(this, out cookie);
								ppTI.ReleaseTypeAttr(typeAttr);
								connection = con;
								break;
							}
						}

						ppTI.ReleaseTypeAttr(typeAttr);
					}
				}
				catch (COMException cme)
				{
				}
			}

			if (connection == null)
				throw new Error("Failed to connect dispatcher to COM interface.");
		}

		~Dispatcher()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public int GetIDsOfNames(Guid riid, string[] names, int cNames, int lcid, int[] rgDispId) => E_NOTIMPL;

		public int GetTypeInfo(int iTInfo, int lcid, out ct.ITypeInfo? ppTInfo)
		{ ppTInfo = null; return E_NOTIMPL; }

		public int GetTypeInfoCount(out int pctinfo)
		{ pctinfo = 0; return 0; }

		//int Invoke(int dispIdMember, Guid riid, int lcid, ct.INVOKEKIND wFlags, ref ct.DISPPARAMS pDispParams, IntPtr pvarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		public int Invoke(int dispIdMember,
						  [MarshalAs(UnmanagedType.LPStruct)]
						  Guid riid,
						  int lcid,
						  ct.INVOKEKIND wFlags,
						  ref ct.DISPPARAMS pDispParams,
						  IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			var args = pDispParams.cArgs > 0 ? Marshal.GetObjectsForNativeVariants(pDispParams.rgvarg, pDispParams.cArgs) : null;
			var names = new string[1];
			typeInfo.GetNames(dispIdMember, names, 1, out var pcNames);
			var evt = new DispatcherEventArgs(dispIdMember, names[0], args);
			OnEvent(this, evt);
			var result = evt.Result;

			if (pVarResult != IntPtr.Zero)
			{
				Marshal.GetNativeVariantForObject(result, pVarResult);
			}

			return 0;
		}

		CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out IntPtr ppv)
		{
			if (iid == typeof(IDispatch).GUID || iid == InterfaceId)
			{
				ppv = Marshal.GetComInterfaceForObject(this, typeof(IDispatch), CustomQueryInterfaceMode.Ignore);
				return CustomQueryInterfaceResult.Handled;
			}

			ppv = IntPtr.Zero;
			return iid == IID_IManagedObject ? CustomQueryInterfaceResult.Failed : CustomQueryInterfaceResult.NotHandled;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				var connection = Interlocked.Exchange(ref this.connection, null);

				if (connection != null)
				{
					connection.Unadvise(cookie);
					cookie = 0;
					_ = Marshal.ReleaseComObject(connection);
				}

				disposedValue = true;
			}
		}

		protected virtual void OnEvent(object sender, DispatcherEventArgs e) => EventReceived?.Invoke(sender, e);

		internal event EventHandler<DispatcherEventArgs> EventReceived;
	}

	internal class DispatcherEventArgs : EventArgs
	{
		internal object[] Arguments { get; }

		internal int DispId { get; }

		internal string Name { get; }

		internal object Result { get; set; }

		internal DispatcherEventArgs(int dispId, string name, params object[] arguments)
		{
			DispId = dispId;
			Name = name;
			Arguments = arguments ?? System.Array.Empty<object>();
		}
	}
}