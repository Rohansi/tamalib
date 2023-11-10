/*
 * TamaLIB - A hardware agnostic Tamagotchi P1 emulation library
 *
 * Copyright (C) 2021 Jean-Christophe Rona <jc@rona.fr>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
#ifndef _CPU_H_
#define _CPU_H_

#include "hal.h"

#define MEMORY_SIZE				4096 // 4096 x 4 bits (640 x 4 bits of RAM)

#define MEM_RAM_ADDR				0x000
#define MEM_RAM_SIZE				0x280
#define MEM_DISPLAY1_ADDR			0xE00
#define MEM_DISPLAY1_SIZE			0x050
#define MEM_DISPLAY2_ADDR			0xE80
#define MEM_DISPLAY2_SIZE			0x050
#define MEM_IO_ADDR				0xF00
#define MEM_IO_SIZE				0x080

/* Define this if you want to reduce the footprint of the memory buffer from 4096 u4_t (most likely bytes)
 * to 464 u8_t (bytes for sure), while increasing slightly the number of operations needed to read/write from/to it.
 */
//#define LOW_FOOTPRINT

#ifdef LOW_FOOTPRINT
/* Invalid memory areas are not buffered to reduce the footprint of the library in memory */
#define MEM_BUFFER_SIZE				(MEM_RAM_SIZE + MEM_DISPLAY1_SIZE + MEM_DISPLAY2_SIZE + MEM_IO_SIZE)/2

/* Maps the CPU memory to the memory buffer */
#define RAM_TO_MEMORY(n)			((n - MEM_RAM_ADDR)/2)
#define DISP1_TO_MEMORY(n)			((n - MEM_DISPLAY1_ADDR + MEM_RAM_SIZE)/2)
#define DISP2_TO_MEMORY(n)			((n - MEM_DISPLAY2_ADDR + MEM_RAM_SIZE + MEM_DISPLAY1_SIZE)/2)
#define IO_TO_MEMORY(n)				((n - MEM_IO_ADDR + MEM_RAM_SIZE + MEM_DISPLAY1_SIZE + MEM_DISPLAY2_SIZE)/2)

#define SET_RAM_MEMORY(buffer, n, v)		{buffer[RAM_TO_MEMORY(n)] = (buffer[RAM_TO_MEMORY(n)] & ~(0xF << (((n) % 2) << 2))) | ((v) & 0xF) << (((n) % 2) << 2);}
#define SET_DISP1_MEMORY(buffer, n, v)		{buffer[DISP1_TO_MEMORY(n)] = (buffer[DISP1_TO_MEMORY(n)] & ~(0xF << (((n) % 2) << 2))) | ((v) & 0xF) << (((n) % 2) << 2);}
#define SET_DISP2_MEMORY(buffer, n, v)		{buffer[DISP2_TO_MEMORY(n)] = (buffer[DISP2_TO_MEMORY(n)] & ~(0xF << (((n) % 2) << 2))) | ((v) & 0xF) << (((n) % 2) << 2);}
#define SET_IO_MEMORY(buffer, n, v)		{buffer[IO_TO_MEMORY(n)] = (buffer[IO_TO_MEMORY(n)] & ~(0xF << (((n) % 2) << 2))) | ((v) & 0xF) << (((n) % 2) << 2);}
#define SET_MEMORY(buffer, n, v)		{if ((n) < (MEM_RAM_ADDR + MEM_RAM_SIZE)) { \
							SET_RAM_MEMORY(buffer, n, v); \
						} else if ((n) < MEM_DISPLAY1_ADDR) { \
							/* INVALID_MEMORY */ \
						} else if ((n) < (MEM_DISPLAY1_ADDR + MEM_DISPLAY1_SIZE)) { \
							SET_DISP1_MEMORY(buffer, n, v); \
						} else if ((n) < MEM_DISPLAY2_ADDR) { \
							/* INVALID_MEMORY */ \
						} else if ((n) < (MEM_DISPLAY2_ADDR + MEM_DISPLAY2_SIZE)) { \
							SET_DISP2_MEMORY(buffer, n, v); \
						} else if ((n) < MEM_IO_ADDR) { \
							/* INVALID_MEMORY */ \
						} else if ((n) < (MEM_IO_ADDR + MEM_IO_SIZE)) { \
							SET_IO_MEMORY(buffer, n, v); \
						} else { \
							/* INVALID_MEMORY */ \
						}}

#define GET_RAM_MEMORY(buffer, n)		((buffer[RAM_TO_MEMORY(n)] >> (((n) % 2) << 2)) & 0xF)
#define GET_DISP1_MEMORY(buffer, n)		((buffer[DISP1_TO_MEMORY(n)] >> (((n) % 2) << 2)) & 0xF)
#define GET_DISP2_MEMORY(buffer, n)		((buffer[DISP2_TO_MEMORY(n)] >> (((n) % 2) << 2)) & 0xF)
#define GET_IO_MEMORY(buffer, n)		((buffer[IO_TO_MEMORY(n)] >> (((n) % 2) << 2)) & 0xF)
#define GET_MEMORY(buffer, n)			((buffer[ \
							((n) < (MEM_RAM_ADDR + MEM_RAM_SIZE)) ? RAM_TO_MEMORY(n) : \
							((n) < MEM_DISPLAY1_ADDR) ? 0 : \
							((n) < (MEM_DISPLAY1_ADDR + MEM_DISPLAY1_SIZE)) ? DISP1_TO_MEMORY(n) : \
							((n) < MEM_DISPLAY2_ADDR) ? 0 : \
							((n) < (MEM_DISPLAY2_ADDR + MEM_DISPLAY2_SIZE)) ? DISP2_TO_MEMORY(n) : \
							((n) < MEM_IO_ADDR) ? 0 : \
							((n) < (MEM_IO_ADDR + MEM_IO_SIZE)) ? IO_TO_MEMORY(n) : 0 \
						] >> (((n) % 2) << 2)) & 0xF)

#define MEM_BUFFER_TYPE				u8_t
#else
#define MEM_BUFFER_SIZE				MEMORY_SIZE

#define SET_MEMORY(buffer, n, v)		{buffer[n] = v;}
#define SET_RAM_MEMORY(buffer, n, v)		SET_MEMORY(buffer, n, v)
#define SET_DISP1_MEMORY(buffer, n, v)		SET_MEMORY(buffer, n, v)
#define SET_DISP2_MEMORY(buffer, n, v)		SET_MEMORY(buffer, n, v)
#define SET_IO_MEMORY(buffer, n, v)		SET_MEMORY(buffer, n, v)

#define GET_MEMORY(buffer, n)			(buffer[n])
#define GET_RAM_MEMORY(buffer, n)		GET_MEMORY(buffer, n)
#define GET_DISP1_MEMORY(buffer, n)		GET_MEMORY(buffer, n)
#define GET_DISP2_MEMORY(buffer, n)		GET_MEMORY(buffer, n)
#define GET_IO_MEMORY(buffer, n)		GET_MEMORY(buffer, n)

#define MEM_BUFFER_TYPE				u4_t
#endif

#define INPUT_PORT_NUM				2

/* Pins (TODO: add other pins) */
typedef enum {
	PIN_K00 = 0x0,
	PIN_K01 = 0x1,
	PIN_K02 = 0x2,
	PIN_K03 = 0x3,
	PIN_K10 = 0X4,
	PIN_K11 = 0X5,
	PIN_K12 = 0X6,
	PIN_K13 = 0X7,
} pin_t;

typedef enum {
	PIN_STATE_LOW = 0,
	PIN_STATE_HIGH = 1,
} pin_state_t;

typedef enum {
	INT_PROG_TIMER_SLOT = 0,
	INT_SERIAL_SLOT = 1,
	INT_K10_K13_SLOT = 2,
	INT_K00_K03_SLOT = 3,
	INT_STOPWATCH_SLOT = 4,
	INT_CLOCK_TIMER_SLOT = 5,
	INT_SLOT_NUM,
} int_slot_t;

typedef struct {
	u4_t factor_flag_reg;
	u4_t mask_reg;
	bool_t triggered; /* 1 if triggered, 0 otherwise */
	u8_t vector;
} interrupt_t;

typedef struct {
	u4_t states;
} input_port_t;

typedef struct {
	u13_t pc;
	u13_t next_pc;
	u12_t x;
	u12_t y;
	u4_t a;
	u4_t b;
	u5_t np;
	u8_t sp;
	u4_t flags;
	const u12_t *g_program;
	MEM_BUFFER_TYPE memory[MEM_BUFFER_SIZE];
	interrupt_t interrupts[INT_SLOT_NUM];
	input_port_t inputs[INPUT_PORT_NUM];
	hal_t g_hal;
	
	u32_t call_depth;
	u32_t clk_timer_timestamp;
	u32_t prog_timer_timestamp;
	bool_t prog_timer_enabled;
	u8_t prog_timer_data;
	u8_t prog_timer_rld;

	u32_t tick_counter;
	u32_t ts_freq;
	u8_t speed_ratio;
	timestamp_t ref_ts;
	u8_t previous_cycles;
	u8_t overflow_cycles;
} cpu_t;

void cpu_set_speed(cpu_t *cpu, u8_t speed);

u32_t cpu_get_depth(const cpu_t *cpu);

void cpu_set_input_pin(cpu_t *cpu, pin_t pin, pin_state_t state);

void cpu_sync_ref_timestamp(cpu_t *cpu);

void cpu_refresh_hw(cpu_t *cpu);

void cpu_reset(cpu_t *cpu);

void cpu_init(cpu_t *cpu, const u12_t *program);
void cpu_release(cpu_t *cpu);

u8_t cpu_step(cpu_t *cpu);

#endif /* _CPU_H_ */
