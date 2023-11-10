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
#ifndef _HAL_H_
#define _HAL_H_

#include "hal_types.h"

#ifndef NULL
	#define NULL 0
#endif

typedef void (*hal_halt_handler_t)(void);
typedef void (*hal_set_lcd_matrix_handler_t)(u8_t x, u8_t y, bool_t val);
typedef void (*hal_set_lcd_icon_handler_t)(u8_t icon, bool_t val);
typedef void (*hal_set_frequency_handler_t)(u32_t freq);
typedef void (*hal_play_frequency_handler_t)(bool_t en);

/* The Hardware Abstraction Layer
 * NOTE: This structure acts as an abstraction layer between TamaLIB and the OS/SDK.
 * All pointers MUST be implemented, but some implementations can be left empty.
 */
typedef struct {

	/* What to do if the CPU has halted
	 */
	hal_halt_handler_t halt;

	/* Clock related functions
	 * NOTE: Timestamps granularity is configured with tamalib_create(), an accuracy
	 * of ~30 us (1/32768) is required for a cycle accurate emulation.
	 */
	//void (*sleep_until)(timestamp_t ts);
	//timestamp_t (*get_timestamp)(void);

	/* Screen related functions
	 * NOTE: In case of direct hardware access to pixels, the set_XXXX() functions
	 * (called for each pixel/icon update) can directly drive them, otherwise they
	 * should just store the data in a buffer and let update_screen() do the actual
	 * rendering (at 30 fps).
	 */
	//void (*update_screen)(void);
	hal_set_lcd_matrix_handler_t set_lcd_matrix;
	hal_set_lcd_icon_handler_t set_lcd_icon;

	/* Sound related functions
	 * NOTE: set_frequency() changes the output frequency of the sound in dHz, while
	 * play_frequency() decides whether the sound should be heard or not.
	 */
	hal_set_frequency_handler_t set_frequency;
	hal_play_frequency_handler_t play_frequency;

	/* Event handler from the main app (if any)
	 * NOTE: This function usually handles button related events, states loading/saving ...
	 */
	//int (*handler)(void);
} hal_t;

#endif /* _HAL_H_ */
