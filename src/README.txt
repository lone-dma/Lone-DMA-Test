*** This tool is run from your Attack PC (2nd PC) ***

The latest tool(s) can be downloaded from https://lone-dma.org/


====== INSTRUCTIONS ======

1) Make sure the FTDI Driver is installed on your system, and you have the USB Cable connected to your FPGA on the DATA port. See the bundled FTD3XXDriver_WHQLCertified_1.3.0.10_Installer file.
	NOTE: Be sure you use the 1.3.x version, the 1.4 version does not work for most people!

2) If you have issues starting this program, try installing the Microsoft Visual C++ Redistributable @ https://aka.ms/vs/17/release/vc_redist.x64.exe


DMA Troubleshooting - Consolidated Steps
=========================================

Quick Prechecks
---------------
- Reboot BOTH machines (Target PC and Attack PC (2nd PC)) if anything seems hung or after a fast reboot/sleep.
- Verify ALL setup steps were completed:
  - BIOS: Disable virtualization features (Intel: VT-d/IOMMU; AMD: SVM/IOMMU). Disable NX-Bit if present. Set PCIe slot for the DMA card to Gen1 if needed.
  - Windows (Target PC): Memory Integrity / Core Isolation OFF.
  - Hardware: DMA card seated firmly; USB cable connected to the DATA port on the DMA card.
  - Attack PC (2nd PC): FTDI (FT601/D3XX) driver installed (FTD3XXDriver_WHQLCertified_1.3.0.10_Installer) and device present in Device Manager.

Common Errors & Fixes
---------------------
1) ERROR - [FAIL] VMM INIT FAILED
   - Cause: DMA card/connection hung (often after fast reboot/sleep).
   - Fix: Reboot (cold boot from shutdown) BOTH Target PC and Attack PC (2nd PC). Double‑check ALL setup steps above.

2) ERROR - DEVICE: FPGA: ERROR: Unable to connect to USB/FT601 device [0,v0.0,0000]
   - Fixes:
     - Install FTDI driver on Attack PC (2nd PC) (FTD3XXDriver_WHQLCertified_1.3.0.10_Installer).
     - Ensure USB cable is in the DATA PORT on the DMA card.

3) Other Common Errors
   a) Failed to retrieve Physical Memory Map!
      - Troubleshoot USB connection (see USB steps below).
      - If all else fails, reinstall Windows on Target PC.

   b) [SYMBOL] Offline symbols unavailable - file 'info.db' not found.
      - Ensure the tool folder was fully unzipped.
      - Confirm symsrv.dll and dbghelp.dll are present.
      - Verify Internet connectivity/firewall is not blocking symbol downloads.
      - If needed, download latest info.db from MemProcFS releases and place it beside the tool for offline symbols.

   c) FPGA: TINY PCIe TLP Algorithm auto-selected!
      - Indicates low link speed (firmware or weak USB connection).
      - Reseat/secure USB cable; try different USB ports.

   d) [CORE] Initialization Failed. Unable to locate valid DTB / Unable to auto‑identify operating system.
      - Reapply BIOS settings (disable virtualization, etc.).
      - Troubleshoot USB connection.
      - Last resort: Reinstall Windows on Target PC.

DMA Test Tool Failures (FAIL / Low Metrics)
-------------------------------------------
- Typical thresholds:
  - Latency (Mem/Sec): < 3000 = FAIL; 3000+ Acceptable; 4000+ Good; 6000+ Excellent.
  - Throughput (MB/s): < 100 = FAIL; 100+ Acceptable; 150+ Good; 200+ Excellent.
- Most common cause: Poor USB connection.
  USB Troubleshooting:
    - Use a data‑rated USB 3.0 cable (not charge‑only); seat securely on both ends.
    - Try different USB ports and/or cables.
    - Ensure DMA card bracket isn’t impeding a solid USB plug fit.
    - On Attack PC (2nd PC): set Power Plan to High Performance.
    - Under‑spec Attack PC (2nd PC)s may reduce performance.
- Less common cause: DMA firmware.
- If close to thresholds, performance may be adequate.

Failed to read virtual memory in certain regions
------------------------------------------------------------------
- Common cause: Paged‑Out Memory on Target PC under high memory pressure.
  - Virtual memory gets moved to the Page File (on disk), which the DMA card cannot read.
  - Fixes:
    - Add more RAM to the Target PC.
    - (Not recommended) Disable Windows Page File—risk of BSOD if commits exceed RAM.

FTDI Driver Verification (Attack PC (2nd PC))
-----------------------------------
- In Device Manager, confirm the FTDI/FT601 device is present.
- If device not detected: reseat USB cable and/or reboot Attack PC (2nd PC).

Last Resort Steps
-----------------
- Reinstall Windows on the Target PC if persistent memory map/DTB/OS ID issues remain after USB and BIOS checks.
- Reflash/verify DMA card firmware if link speed/connection instability persists despite cable/port changes.

Testing Flow (Attack PC (2nd PC))
-----------------------
1) Unzip the test package completely; read README.txt.
2) Run the test tool and perform the FULL test.
3) Verify PASS message and review metrics against thresholds above.
4) If failing, work through the USB/BIOS/driver checks and retest.

Notes
-----
- Always power‑cycle both PCs after sleep/fast reboots to clear a hung DMA link.
- Keep iterating on USB cabling/ports and physical seating—most users recover performance through these steps.